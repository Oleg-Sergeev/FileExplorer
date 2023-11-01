using System.Collections.Concurrent;
using Serilog;

namespace FileExplorer.Services;

public class BackgroundTaskHandler : IHostedService
{
    private readonly Serilog.ILogger _logger = Log.ForContext<BackgroundTaskHandler>();
    private readonly ConcurrentQueue<Task> _queue = new();
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly SemaphoreSlim _semaphore = new(2);

    private Task? _loopTask;

    public CancellationToken Token => _cancellationTokenSource.Token;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _loopTask = Task.Run(LoopQueue, _cancellationTokenSource.Token);

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.Information("Stopping task handler...");
        _cancellationTokenSource.Cancel();

        if (_loopTask is not null)
        {
            try
            {
                await _loopTask;

                _logger.Debug("Loop task finished");
            }
            catch (OperationCanceledException)
            {
                _logger.Debug("Loop task canceled");
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex, "Loop task faulted");
            }
        }

        foreach (var task in _queue)
        {
            _logger.Verbose("Finishing queued task {id}...", task.Id);

            try
            {
                await task;

                _logger.Verbose("Queued task {id} finished", task.Id);
            }
            catch (OperationCanceledException)
            {
                _logger.Verbose("Queued task {id} canceled", task.Id);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Queued task {id} faulted", task.Id);
            }
        }

        _queue.Clear();

        _cancellationTokenSource.Dispose();


        _logger.Information("Task handler stopped");
    }

    public void EnqueueTask(Func<Task> action)
    {
        var task = Task.Run(async () =>
        {
            _logger.Verbose("Task {id} preparing", action.GetHashCode());
            await _semaphore.WaitAsync();

            _logger.Verbose("Task {id} processing", action.GetHashCode());
            await action();

            _logger.Verbose("Task {id} processed", action.GetHashCode());
            _semaphore.Release();

        }, _cancellationTokenSource.Token);

        _queue.Enqueue(task);
    }

    private async Task LoopQueue()
    {
        try
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                if (_queue.TryPeek(out var task) && task.IsCompleted && _queue.TryDequeue(out task))
                {
                    try
                    {
                        await task;
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.Verbose("Task {id} was canceled", task.Id);
                    }
                    catch (Exception e)
                    {
                        _logger.Error(e, "Error in queued task {id}", task.Id);
                    }
                }

                await Task.Delay(1000);
            }
        }
        catch (Exception e)
        {
            _logger.Error(e, "Error in queue loop");
        }
    }
}
