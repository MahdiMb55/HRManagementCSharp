using HRManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagement.Infrastructure.Persistence.Configurations;

public sealed class ResponsibilityConfiguration : IEntityTypeConfiguration<Responsibility>
{
    public void Configure(EntityTypeBuilder<Responsibility> builder)
    {
        builder.ToTable("Responsibilities");
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.Title).HasMaxLength(150).IsRequired();
        builder.Property(entity => entity.NormalizedTitle).HasMaxLength(150).IsRequired();
        builder.HasIndex(entity => entity.NormalizedTitle);
        builder.HasIndex(entity => entity.IsActive);
    }
}
