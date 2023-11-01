using Ardalis.Result;
using FileExplorer.Data;
using Microsoft.EntityFrameworkCore;

namespace FileExplorer.Services;

public class FileShareLinkService : IFileShareLinkService
{
    private readonly FilesDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public FileShareLinkService(FilesDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }


    async Task<Result<string>> IFileShareLinkService.CreateOneTimeShareLinkAsync(string userId, IEnumerable<int> fileIds)
    {
        var hasUser = await _context.Users.AnyAsync(u => u.Id == userId);

        if (!hasUser)
            return Result.NotFound("User was not found");

        var hashSetIds = fileIds.ToHashSet();

        var userFileIds = await _context.UserFiles
            .AsNoTracking()
            .Where(uf => uf.UserId == userId && hashSetIds.Contains(uf.Id))
            .Select(uf => uf.Id)
            .ToArrayAsync();

        if (userFileIds.Length == 0)
            return Result.NotFound("Files were not found");

        var guid = Guid.NewGuid();

        var request = _httpContextAccessor.HttpContext!.Request;

        var url = $"{request.Scheme}://{request.Host}/files/share/{guid}";

        var userLink = new OneTimeShareLink
        {
            Id = guid,
            UserId = userId,
            Url = url,
            FileIds = hashSetIds
        };

        _context.OneTimeShareLinks.Add(userLink);

        await _context.SaveChangesAsync();

        if (userFileIds.Length != hashSetIds.Count)
            return Result.Success(url, $"{hashSetIds.Count - userFileIds.Length} file(s) not found");

        return Result.Success(url);
    }
}
