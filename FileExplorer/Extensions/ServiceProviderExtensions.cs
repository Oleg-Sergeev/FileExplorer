namespace FileExplorer.Extensions;

public static class ServiceProviderExtensions
{
    public static TWorkerType GetRequiredHostedService<TWorkerType>(this IServiceProvider serviceProvider) where TWorkerType : IHostedService
        => serviceProvider
        .GetServices<IHostedService>()
        .OfType<TWorkerType>()
        .First();
}