using System.Security.Claims;
using FileExplorer.Services;
using Microsoft.AspNetCore.Mvc;

namespace FileExplorer.Endpoints;

public static class FileEndpoints
{
    public static WebApplication MapFileEndpoints(this WebApplication app)
    {
        var fileGroup = app.MapGroup("/files")
            .RequireAuthorization();

        fileGroup.MapGet("/get", GetFileInfosAsync);
        fileGroup.MapGet("/download", GetZipAsync);
        fileGroup.MapGet("/download/{id}", GetFileAsync);
        fileGroup.MapPost("/upload", UploadFilesAsync);

        return app;
    }

    private static async Task<IResult> GetFileInfosAsync(HttpContext context, [FromServices] IUserFileService userFileService)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
            return Results.Unauthorized();

        var files = await userFileService.GetAsync(userId);

        if (files is null)
            return Results.NotFound();

        return Results.Json(files);
    }

    private static async Task<IResult> GetFileAsync(HttpContext context, [FromServices] IUserFileService userFileService, [FromRoute] int id)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
            return Results.Unauthorized();

        var stream = await userFileService.GetAsync(id, userId);

        if (stream is null)
            return Results.NotFound();

        return Results.File(stream);
    }
    
    private static async Task<IResult> GetZipAsync(HttpContext context, [FromServices] IUserFileService userFileService, int[] ids)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
            return Results.Unauthorized();

        var stream = await userFileService.GetZipAsync(ids, userId);

        if (stream is null)
            return Results.NotFound();

        return Results.File(stream, "application/zip", "Zip.zip");
    }

    private static async Task<IResult> UploadFilesAsync(HttpContext context, [FromServices] IUserFileService userFileService, [FromForm] IFormFileCollection files)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
            return Results.Unauthorized();

        await userFileService.UploadAsync(files, userId);

        return Results.Ok();
    }
}
