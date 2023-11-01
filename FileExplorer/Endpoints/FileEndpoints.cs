using System.Security.Claims;
using Ardalis.Result.AspNetCore;
using FileExplorer.Extensions;
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

        fileGroup.MapGet("/share/{id:guid}", GetFilesByOneTimeLinkAsync)
            .AllowAnonymous();

        fileGroup.MapGet("/progress/{id:guid}", GetFileGroupProgress);
       
        return app;
    }


    private static async Task<IResult> GetFileInfosAsync(HttpContext context, [FromServices] IUserFileService userFileService)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
            return Results.Unauthorized();

        var filesRes = await userFileService.GetAsync(userId);

        return filesRes.ToMinimalApiResult();
    }

    private static async Task<IResult> GetFileAsync(HttpContext context, [FromServices] IUserFileService userFileService, [FromRoute] int id)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
            return Results.Unauthorized();

        var streamRes = await userFileService.GetAsync(id, userId);

        if (!streamRes.IsSuccess)
            return streamRes.ToMinimalApiResult();

        var value = streamRes.Value;

        return Results.File(value.Stream, value.Name.GetContentType(), value.Name);
    }

    private static async Task<IResult> GetZipAsync(HttpContext context, [FromServices] IUserFileService userFileService, int[] ids)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
            return Results.Unauthorized();

        var streamRes = await userFileService.GetZipAsync(ids, userId);

        if (!streamRes.IsSuccess)
            return streamRes.ToMinimalApiResult();

        var value = streamRes.Value;

        return Results.File(value.Stream, value.Name.GetContentType(), value.Name);
    }

    private static async Task<IResult> UploadFilesAsync(HttpContext context, [FromServices] IUserFileService userFileService, [FromForm] IFormFileCollection files)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
            return Results.Unauthorized();

        var res = await userFileService.UploadAsync(files, userId);

        return res.ToMinimalApiResult();
    }

    private static async Task<IResult> GetFilesByOneTimeLinkAsync([FromServices] IUserFileService userFileService, [FromRoute] Guid id)
    {
        var streamRes = await userFileService.GetZipByOneTimeLinkAsync(id);

        if (!streamRes.IsSuccess)
            return streamRes.ToMinimalApiResult();

        var value = streamRes.Value;

        return Results.File(value.Stream, value.Name.GetContentType(), value.Name);
    }

    private static IResult GetFileGroupProgress([FromServices] IUploadProgressService progressService, [FromRoute] Guid id)
    {
        if (progressService.TryGetFilesProgress(id, out var files))
            return Results.Json(files);

        return Results.NotFound();
    }
}
