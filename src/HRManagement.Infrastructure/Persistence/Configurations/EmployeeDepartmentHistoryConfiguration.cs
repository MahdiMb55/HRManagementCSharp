using HRManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagement.Infrastructure.Persistence.Configurations;

public sealed class EmployeeDepartmentHistoryConfiguration : IEntityTypeConfiguration<EmployeeDepartmentHistory>
{
    public void Configure(EntityTypeBuilder<EmployeeDepartmentHistory> builder)
    {
        builder.ToTable("EmployeeDepartmentHistories");
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.StartDate).HasConversion<DateOnlyTextConverter>().HasColumnType("TEXT");
        builder.Property(entity => entity.EndDate).HasConversion<NullableDateOnlyTextConverter>().HasColumnType("TEXT");
        builder.HasOne<Employee>().WithMany().HasForeignKey(entity => entity.EmployeeId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<EmploymentPeriod>().WithMany().HasForeignKey(entity => entity.EmploymentPeriodId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Department>().WithMany().HasForeignKey(entity => entity.DepartmentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(entity => entity.EmployeeId).IsUnique().HasFilter("EndDate IS NULL");
        builder.HasIndex(entity => new { entity.EmployeeId, entity.StartDate, entity.EndDate });
        builder.HasIndex(entity => new { entity.DepartmentId, entity.StartDate, entity.EndDate });
    }
}
