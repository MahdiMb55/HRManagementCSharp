using HRManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagement.Infrastructure.Persistence.Configurations;

public sealed class EducationRecordConfiguration : IEntityTypeConfiguration<EducationRecord>
{
    public void Configure(EntityTypeBuilder<EducationRecord> builder)
    {
        builder.ToTable("EducationRecords");
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.FieldOfStudy).HasMaxLength(150);
        builder.Property(entity => entity.InstitutionName).HasMaxLength(200);
        builder.HasOne<Employee>().WithMany().HasForeignKey(entity => entity.EmployeeId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(entity => entity.EmployeeId).IsUnique().HasFilter("IsPrimary = 1 AND IsDeleted = 0");
        builder.HasIndex(entity => new { entity.Degree, entity.IsDeleted });
    }
}
