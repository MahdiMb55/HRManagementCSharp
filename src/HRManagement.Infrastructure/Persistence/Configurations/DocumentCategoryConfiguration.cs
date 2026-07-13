using HRManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagement.Infrastructure.Persistence.Configurations;

public sealed class DocumentCategoryConfiguration : IEntityTypeConfiguration<DocumentCategory>
{
    public void Configure(EntityTypeBuilder<DocumentCategory> builder)
    {
        builder.ToTable("DocumentCategories");
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.Name).HasMaxLength(150).IsRequired();
        builder.HasIndex(entity => entity.Name).IsUnique();
        builder.HasIndex(entity => entity.IsActive);
        builder.HasData(SystemDocumentCategories.All.Select(category => new
        {
            category.Id,
            category.Name,
            category.Description,
            IsSystemCategory = true,
            IsActive = true,
            CreatedAtUtc = SystemDocumentCategories.SeedTimestampUtc,
            UpdatedAtUtc = SystemDocumentCategories.SeedTimestampUtc,
        }));
    }
}
