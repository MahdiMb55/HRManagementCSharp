using HRManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagement.Infrastructure.Persistence.Configurations;

public sealed class ManagedFileConfiguration : IEntityTypeConfiguration<ManagedFile>
{
    public void Configure(EntityTypeBuilder<ManagedFile> builder)
    {
        builder.ToTable("ManagedFiles");
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.OriginalFileName).HasMaxLength(255).IsRequired();
        builder.Property(entity => entity.StoredFileName).HasMaxLength(100).IsRequired();
        builder.Property(entity => entity.RelativePath).HasMaxLength(500).IsRequired();
        builder.Property(entity => entity.Extension).HasMaxLength(20).IsRequired();
        builder.Property(entity => entity.MimeType).HasMaxLength(100).IsRequired();
        builder.Property(entity => entity.FileHash).HasMaxLength(64).IsRequired();
        builder.Property(entity => entity.TrashRelativePath).HasMaxLength(500);
        builder.HasIndex(entity => entity.RelativePath).IsUnique();
        builder.HasIndex(entity => entity.FileHash);
        builder.HasIndex(entity => entity.IsInTrash);
    }
}
