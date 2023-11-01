using System.IO.Compression;
using System.Text;
using Ardalis.Result;
using FileExplorer.Data;
using FileExplorer.Models.UserFile;
using Microsoft.EntityFrameworkCore;

namespace FileExplorer.Services;

public class UserFileService : IUserFileService
{
    private readonly FilesDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public UserFileService(FilesDbContext appContext, IWebHostEnvironment environment)
    {
        _context = appContext;
        _environment = environment;
    }


    async Task<Result<IReadOnlyCollection<UserFileResponse>>> IUserFileService.GetAsync(string userId)
    {
        var userFiles = await _context.UserFiles
            .AsNoTracking()
            .Where(uf => uf.UserId == userId)
            .Select(uf => new UserFileResponse(uf.Id, uf.Name, uf.CreatedAt))
            .ToArrayAsync();

        return userFiles;
    }

    async Task<Result<UserFileStreamResponse>> IUserFileService.GetAsync(int id, string userId)
    {
        var userFile = await _context.UserFiles
            .AsNoTracking()
            .Where(uf => uf.Id == id && uf.UserId == userId)
            .FirstOrDefaultAsync();

        if (userFile is null)
            return Result.NotFound("File was not found");

        var path = Path.Combine(_environment.WebRootPath, "Files", userId, userFile.Name);

        if (!File.Exists(path))
            throw new FileNotFoundException($"The file {userFile.Name} ({id}) is contained in the database, but is not physically present");

        var fileStream = File.OpenRead(path);

        UserFileStreamResponse response = new(fileStream, userFile.Name);

        return response;
    }

    async Task<Result<UserFileStreamResponse>> IUserFileService.GetZipAsync(IEnumerable<int> ids, string userId)
    {
        var userFiles = await _context.UserFiles
            .AsNoTracking()
            .Where(uf => uf.UserId == userId && ids.Contains(uf.Id))
            .ToArrayAsync();

        if (userFiles.Length == 0)
            return Result.NotFound("Files were not found");

        var zipStream = GenerateZip(userFiles, Path.Combine(_environment.WebRootPath, "Files", userId));

        zipStream.Position = 0;

        UserFileStreamResponse response = new(zipStream, $"{Guid.NewGuid():n}.zip");

        return response;
    }

    async Task<Result> IUserFileService.UploadAsync(IFormFileCollection uploadFiles, string userId)
    {
        var hasUser = await _context.Users.AnyAsync(u => u.Id == userId);

        if (!hasUser)
            return Result.NotFound("User was not found");

        var userFilesFolder = Path.Combine(_environment.WebRootPath, "Files", userId);

        if (!Directory.Exists(userFilesFolder))
            Directory.CreateDirectory(userFilesFolder);

        foreach (var uploadFile in uploadFiles)
        {
            var filePath = Path.Combine(userFilesFolder, uploadFile.FileName);

            using MemoryStream ms = new();
            await uploadFile.CopyToAsync(ms);
            await File.WriteAllBytesAsync(filePath, ms.ToArray());

            var userFile = new UserFile(userId, uploadFile.FileName);

            _context.UserFiles.Add(userFile);
        }

        await _context.SaveChangesAsync();

        return Result.Success();
    }

    async Task<Result<UserFileStreamResponse>> IUserFileService.GetZipByOneTimeLinkAsync(Guid guid)
    {
        var link = await _context.OneTimeShareLinks
            .FirstOrDefaultAsync(link => link.Id == guid);

        if (link is null)
            return Result.NotFound("Link was not found");

        if (link.IsUsed)
            return Result.Conflict("Link has already been used");

        var fileIdsHashSet = link.FileIds.ToHashSet();

        var userFiles = await _context.UserFiles
            .AsNoTracking()
            .Where(uf => fileIdsHashSet.Contains(uf.Id))
            .ToArrayAsync();

        var zipStream = GenerateZip(userFiles, Path.Combine(_environment.WebRootPath, "Files", link.UserId));

        zipStream.Position = 0;

        link.IsUsed = true;
        link.UsedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        UserFileStreamResponse response = new(zipStream, $"shared_{Guid.NewGuid():n}.zip");

        return response;
    }


    private static MemoryStream GenerateZip(IEnumerable<UserFile> userFiles, string userFolderPath)
    {
        var outputStream = new MemoryStream();

        using var archive = new ZipArchive(outputStream, ZipArchiveMode.Create, true, Encoding.GetEncoding("cp866"));

        foreach (var userFile in userFiles)
        {
            var path = Path.Combine(userFolderPath, userFile.Name);

            if (!File.Exists(path))
                throw new FileNotFoundException($"The file {userFile.Name} ({userFile.Id}) is contained in the database, but is not physically present");

            var entry = archive.CreateEntry(userFile.Name);

            using var entryStream = entry.Open();

            using var fileStream = File.OpenRead(path);

            fileStream.CopyTo(entryStream);
        }

        return outputStream;
    }
}