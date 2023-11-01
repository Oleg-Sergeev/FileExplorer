using System.Diagnostics.CodeAnalysis;

namespace FileExplorer.Services;
public interface IUploadProgressService
{
    bool AddFile(Guid fileGroupId, string fileName);
    bool RemoveFileGroup(Guid fileGroupId);
    bool TryGetFilesProgress(Guid fileGroupId, [NotNullWhen(true)] out Dictionary<string, double>? files);
    bool UpdateProgress(Guid fileGroupId, string fileName, double value);
}