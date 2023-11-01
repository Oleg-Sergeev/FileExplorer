using Ardalis.Result;
using FileExplorer.Models.UserFile;

namespace FileExplorer.Services;

public interface IUserFileService
{
    Task<Result<IReadOnlyCollection<UserFileResponse>>> GetAsync(string userId);
    Task<Result<UserFileStreamResponse>> GetAsync(int id, string userId);
    Task<Result<UserFileStreamResponse>> GetZipAsync(IEnumerable<int> ids, string userId);
    Task<Result<UserFileStreamResponse>> GetZipByOneTimeLinkAsync(Guid guid);
    Task<Result<Guid>> UploadAsync(IFormFileCollection uploadFiles, string userId);
}