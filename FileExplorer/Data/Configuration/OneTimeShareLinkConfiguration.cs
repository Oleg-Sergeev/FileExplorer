using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileExplorer.Data.Configuration;

public class OneTimeShareLinkConfiguration : IEntityTypeConfiguration<OneTimeShareLink>
{
    public void Configure(EntityTypeBuilder<OneTimeShareLink> builder)
    {
        builder.Property(link => link.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(link => link.Id)
            .ValueGeneratedNever();

        builder.Property(link => link.FileIds)
            .HasConversion(
                ids => string.Join(',', ids),
                str => new List<int>(str.Split(',', StringSplitOptions.None).Select(int.Parse))
            );
    }
}
