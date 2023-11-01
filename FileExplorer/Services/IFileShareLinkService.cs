using Ardalis.Result;

namespace FileExplorer.Services;
public interface IFileShareLinkService
{
    Task<Result<string>> CreateOneTimeShareLinkAsync(string userId, IEnumerable<int> fileIds);
}