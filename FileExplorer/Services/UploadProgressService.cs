using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace FileExplorer.Services;

public class UploadProgressService : IUploadProgressService
{
    private readonly ConcurrentDictionary<Guid, Dictionary<string, double>> _currentUploads = new();


    public bool TryGetFilesProgress(Guid fileGroupId, [NotNullWhen(true)] out Dictionary<string, double>? files)
        => _currentUploads.TryGetValue(fileGroupId, out files);

    public bool AddFile(Guid fileGroupId, string fileName)
    {
        var files = _currentUploads.GetOrAdd(fileGroupId, new Dictionary<string, double>());

        return files.TryAdd(fileName, 0f);
    }

    public bool UpdateProgress(Guid fileGroupId, string fileName, double value)
    {
        if (_currentUploads.TryGetValue(fileGroupId, out var files) && files.ContainsKey(fileName))
        {
            files[fileName] = value;

            return true;
        }

        return false;
    }

    public bool RemoveFileGroup(Guid fileGroupId)
        => _currentUploads.Remove(fileGroupId, out _);
}
