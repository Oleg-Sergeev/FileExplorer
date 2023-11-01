using Microsoft.AspNetCore.StaticFiles;

namespace FileExplorer.Extensions;

public static class ContentTypeExtensions
{
    public static string GetContentType(this string filePath)
    {
        const string DefaultContentType = "application/octet-stream";

        var provider = new FileExtensionContentTypeProvider();

        if (!provider.TryGetContentType(filePath, out string? contentType))
            contentType = DefaultContentType;

        return contentType;
    }
}
