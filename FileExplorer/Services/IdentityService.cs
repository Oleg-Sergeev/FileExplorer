using FileExplorer.Data;
using FileExplorer.Models;
using Microsoft.AspNetCore.Identity;

namespace FileExplorer.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public IdentityService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    async Task<IResult> IIdentityService.LogInAsync(LogIn login)
    {
        var user = await _userManager.FindByNameAsync(login.Name);

        if (user is not null)
        {
            var loginRes = await _signInManager.PasswordSignInAsync(login.Name, login.Password, true, false);

            if (loginRes.Succeeded)
                return TypedResults.Ok();

            return TypedResults.Unauthorized();
        }

        user = new()
        {
            UserName = login.Name
        };

        var createRes = await _userManager.CreateAsync(user, login.Password);

        if (!createRes.Succeeded)
            return TypedResults.BadRequest(createRes.Errors);

        var signInRes = await _signInManager.PasswordSignInAsync(user, login.Password, true, false);

        if (!signInRes.Succeeded)
            return TypedResults.Unauthorized();

        return TypedResults.Ok();
    }


    async Task<IResult> IIdentityService.LogOutAsync()
    {
        await _signInManager.SignOutAsync();

        return TypedResults.Ok();
    }
}
