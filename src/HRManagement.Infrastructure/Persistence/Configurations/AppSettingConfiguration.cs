using HRManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagement.Infrastructure.Persistence.Configurations;

public sealed class AppSettingConfiguration : IEntityTypeConfiguration<AppSetting>
{
    public void Configure(EntityTypeBuilder<AppSetting> builder)
    {
        builder.ToTable("AppSettings");
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.Key).HasMaxLength(150).IsRequired();
        builder.Property(entity => entity.Value).IsRequired();
        builder.Property(entity => entity.ValueType).HasMaxLength(50).IsRequired();
        builder.HasIndex(entity => entity.Key).IsUnique();
    }
}
