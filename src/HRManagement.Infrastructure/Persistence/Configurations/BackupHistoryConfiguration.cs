using HRManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagement.Infrastructure.Persistence.Configurations;

public sealed class BackupHistoryConfiguration : IEntityTypeConfiguration<BackupHistory>
{
    public void Configure(EntityTypeBuilder<BackupHistory> builder)
    {
        builder.ToTable("BackupHistories");
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.FilePath).HasMaxLength(1000).IsRequired();
        builder.Property(entity => entity.ErrorMessage).HasMaxLength(2000);
        builder.HasIndex(entity => entity.StartedAtUtc);
        builder.HasIndex(entity => new { entity.BackupType, entity.WasSuccessful });
    }
}
