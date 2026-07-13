using HRManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagement.Infrastructure.Persistence.Configurations;

public sealed class EmploymentPeriodConfiguration : IEntityTypeConfiguration<EmploymentPeriod>
{
    public void Configure(EntityTypeBuilder<EmploymentPeriod> builder)
    {
        builder.ToTable("EmploymentPeriods");
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.HireDate).HasConversion<DateOnlyTextConverter>().HasColumnType("TEXT");
        builder.Property(entity => entity.EndDate).HasConversion<NullableDateOnlyTextConverter>().HasColumnType("TEXT");
        builder.HasOne<Employee>().WithMany().HasForeignKey(entity => entity.EmployeeId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(entity => entity.EmployeeId).IsUnique().HasFilter("EndDate IS NULL");
        builder.HasIndex(entity => new { entity.EmployeeId, entity.HireDate, entity.EndDate });
    }
}
