using System.Security.Claims;
using Ardalis.Result.AspNetCore;
using FileExplorer.Services;
using Microsoft.AspNetCore.Mvc;

namespace FileExplorer.Endpoints;

public static class ShareLinkEndpoints
{
    public static WebApplication MapShareLinkEndpoints(this WebApplication app)
    {
        var fileGroup = app.MapGroup("/link")
            .RequireAuthorization();

        fileGroup.MapPost("/create/onetime", CreateOneTimeLinkAsync);

        return app;
    }


    private static async Task<IResult> CreateOneTimeLinkAsync(HttpContext context, [FromServices] IFileShareLinkService linkService, int[] ids)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
            return Results.Unauthorized();

        var res = await linkService.CreateOneTimeShareLinkAsync(userId, ids);

        return res.ToMinimalApiResult();
    }
}
