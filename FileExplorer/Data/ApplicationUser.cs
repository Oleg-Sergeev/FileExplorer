using Microsoft.AspNetCore.Identity;

namespace FileExplorer.Data;


public class ApplicationUser : IdentityUser
{
    public IReadOnlyCollection<UserFile> Files { get; set; } = new List<UserFile>();
}

