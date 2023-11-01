using System.IO.Compression;
using System.Text;
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

    async Task<IReadOnlyCollection<GetUserFile>?> IUserFileService.GetAsync(string userId)
    {
        var userFiles = await _context.UserFiles
            .AsNoTracking()
            .Where(uf => uf.UserId == userId)
            .Select(uf => new GetUserFile(uf.Id, uf.Name, uf.CreatedAt))
            .ToArrayAsync();

        if (userFiles.Length == 0)
            return null;

        return userFiles;
    }

    async Task<FileStream?> IUserFileService.GetAsync(int id, string userId)
    {
        var userFile = await _context.UserFiles
            .AsNoTracking()
            .Where(uf => uf.Id == id)
            .FirstOrDefaultAsync();

        if (userFile is null)
            return null;

        if (userFile.UserId != userId)
            return null;

        var path = Path.Combine(_environment.WebRootPath, "Files", userId, userFile.Name);

        if (!File.Exists(path))
            return null;

        var fileStream = File.OpenRead(path);

        return fileStream;
    }

    async Task<MemoryStream?> IUserFileService.GetZipAsync(IEnumerable<int> ids, string userId)
    {
        var userFiles = await _context.UserFiles
            .AsNoTracking()
            .Where(uf => ids.Contains(uf.Id))
            .ToArrayAsync();

        if (userFiles.Length == 0)
            return null;

        if (userFiles.Any(uf => uf.UserId != userId))
            return null;


        var outputStream = new MemoryStream();

        using (var archive = new ZipArchive(outputStream, ZipArchiveMode.Create, true, Encoding.GetEncoding("cp866")))
        {
            foreach (var userFile in userFiles)
            {
                var path = Path.Combine(_environment.WebRootPath, "Files", userId, userFile.Name);

                if (!File.Exists(path))
                    return null;

                var entry = archive.CreateEntry(userFile.Name);

                using var entryStream = entry.Open();

                using var fileStream = File.OpenRead(path);

                fileStream.CopyTo(entryStream);
            }
        }

        outputStream.Position = 0;

        return outputStream;
    }


    async Task IUserFileService.UploadAsync(IFormFileCollection uploadFiles, string userId)
    {
        var hasUser = await _context.Users.AnyAsync(u => u.Id == userId);

        if (!hasUser)
            return;

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
    }
}