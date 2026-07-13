using HRManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagement.Infrastructure.Persistence.Configurations;

public sealed class ContractConfiguration : IEntityTypeConfiguration<Contract>
{
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.ToTable("Contracts");
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.ContractNumber).HasMaxLength(100).IsRequired();
        builder.Property(entity => entity.StartDate).HasConversion<DateOnlyTextConverter>().HasColumnType("TEXT");
        builder.Property(entity => entity.EndDate).HasConversion<NullableDateOnlyTextConverter>().HasColumnType("TEXT");
        builder.HasOne<Employee>().WithMany().HasForeignKey(entity => entity.EmployeeId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<EmploymentPeriod>().WithMany().HasForeignKey(entity => entity.EmploymentPeriodId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(entity => new { entity.EmploymentPeriodId, entity.StartDate, entity.EndDate });
        builder.HasIndex(entity => new { entity.ContractType, entity.IsDeleted });
        builder.HasIndex(entity => entity.ContractNumber);
    }
}
