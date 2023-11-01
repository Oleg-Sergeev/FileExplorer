using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FileExplorer.Data;

public class FilesDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<UserFile> UserFiles => Set<UserFile>();

    public DbSet<OneTimeShareLink> OneTimeShareLinks => Set<OneTimeShareLink>();

    public FilesDbContext(DbContextOptions<FilesDbContext> options)
        : base(options)
    {
    }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
