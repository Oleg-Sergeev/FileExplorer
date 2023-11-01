using System.Net;
using FileExplorer.Filters;
using FileExplorer.Models;
using FileExplorer.Services;
using Microsoft.AspNetCore.Mvc;

namespace FileExplorer.Endpoints;

public static class IdentityEndpoints
{
    public static WebApplication MapIdentityEndpoints(this WebApplication app)
    {
        app.MapPost("/login", LogInAsync)
           .AddEndpointFilter<LogInIsValidFilter>()
           .Produces<BadRequestResult>((int)HttpStatusCode.BadRequest)
           .Produces<UnauthorizedResult>((int)HttpStatusCode.Unauthorized)
           .Produces<OkResult>();

        app.MapGet("/logout", LogOutAsync)
           .RequireAuthorization()
           .Produces<OkResult>();

        return app;
    }

    private static Task<IResult> LogInAsync([FromServices] IIdentityService identityService, [FromBody] LogIn login)
        => identityService.LogInAsync(login);


    private static Task<IResult> LogOutAsync([FromServices] IIdentityService identityService)
        => identityService.LogOutAsync();
}
