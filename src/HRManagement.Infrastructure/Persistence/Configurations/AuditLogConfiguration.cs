using HRManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagement.Infrastructure.Persistence.Configurations;

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.EntityType).HasMaxLength(100).IsRequired();
        builder.Property(entity => entity.Action).HasMaxLength(100).IsRequired();
        builder.Property(entity => entity.Description).HasMaxLength(1000).IsRequired();
        builder.Property(entity => entity.Reason).HasMaxLength(500);
        builder.HasIndex(entity => new { entity.EntityType, entity.EntityId, entity.CreatedAtUtc });
        builder.HasIndex(entity => entity.CreatedAtUtc);
    }
}
