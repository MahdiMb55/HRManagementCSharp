using HRManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagement.Infrastructure.Persistence.Configurations;

public sealed class EmploymentTerminationConfiguration : IEntityTypeConfiguration<EmploymentTermination>
{
    public void Configure(EntityTypeBuilder<EmploymentTermination> builder)
    {
        builder.ToTable("EmploymentTerminations");
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.TerminationDate).HasConversion<DateOnlyTextConverter>().HasColumnType("TEXT");
        builder.Property(entity => entity.Reason).HasMaxLength(500).IsRequired();
        builder.HasOne<EmploymentPeriod>().WithOne().HasForeignKey<EmploymentTermination>(entity => entity.EmploymentPeriodId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(entity => entity.EmploymentPeriodId).IsUnique();
    }
}
