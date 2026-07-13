using HRManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagement.Infrastructure.Persistence.Configurations;

public sealed class EmployeeAccessCardConfiguration : IEntityTypeConfiguration<EmployeeAccessCard>
{
    public void Configure(EntityTypeBuilder<EmployeeAccessCard> builder)
    {
        builder.ToTable("EmployeeAccessCards");
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.CardNumber).HasMaxLength(50).IsRequired();
        builder.Property(entity => entity.StartDate).HasConversion<DateOnlyTextConverter>().HasColumnType("TEXT");
        builder.Property(entity => entity.EndDate).HasConversion<NullableDateOnlyTextConverter>().HasColumnType("TEXT");
        builder.HasOne<Employee>().WithMany().HasForeignKey(entity => entity.EmployeeId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<EmploymentPeriod>().WithMany().HasForeignKey(entity => entity.EmploymentPeriodId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(entity => entity.EmployeeId).IsUnique().HasFilter("EndDate IS NULL");
        builder.HasIndex(entity => entity.CardNumber).IsUnique().HasFilter("EndDate IS NULL");
        builder.HasIndex(entity => new { entity.EmployeeId, entity.StartDate, entity.EndDate });
    }
}
