using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileExplorer.Data.Configuration;

public class UserFileConfiguration : IEntityTypeConfiguration<UserFile>
{
    public void Configure(EntityTypeBuilder<UserFile> builder)
    {
        builder.Property(uf => uf.CreatedAt)
            .HasDefaultValueSql("GETDATE()");
    }
}
