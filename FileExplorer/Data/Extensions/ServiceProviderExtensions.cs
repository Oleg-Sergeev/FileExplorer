using Microsoft.EntityFrameworkCore;

namespace FileExplorer.Data.Extensions;

public static class ServiceProviderExtensions
{
    public static async Task DatabaseMigrateAsync<TDBContext>(this IServiceProvider serviceProvider) where TDBContext : DbContext
    {
        var context = serviceProvider.GetRequiredService<TDBContext>();

        if (!context.Database.IsSqlServer())
            return;

        await context.Database.MigrateAsync();

        await context.SaveChangesAsync();
    }
}
