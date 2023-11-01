using FileExplorer.Models.UserFile;

namespace FileExplorer.Services;
public interface IUserFileService
{
    Task<FileStream?> GetAsync(int id, string userId);
    Task<IReadOnlyCollection<GetUserFile>?> GetAsync(string userId);
    Task<MemoryStream?> GetZipAsync(IEnumerable<int> ids, string userId);
    Task UploadAsync(IFormFileCollection uploadFiles, string userId);
}