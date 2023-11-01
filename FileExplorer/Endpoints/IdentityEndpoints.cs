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
           .AddEndpointFilter<LogInIsValidFilter>();

        app.MapGet("/logout", LogOutAsync)
           .RequireAuthorization();

        return app;
    }

    private static Task<IResult> LogInAsync([FromServices] IIdentityService identityService, [FromBody] LogIn login)
        => identityService.LogInAsync(login);


    private static Task<IResult> LogOutAsync([FromServices] IIdentityService identityService)
        => identityService.LogOutAsync();
}
