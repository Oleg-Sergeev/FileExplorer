using FileExplorer.Models;

namespace FileExplorer.Services;
public interface IIdentityService
{
    Task<IResult> LogInAsync(LogIn login);
    Task<IResult> LogOutAsync();
}