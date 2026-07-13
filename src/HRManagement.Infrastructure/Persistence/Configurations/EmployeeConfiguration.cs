using HRManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagement.Infrastructure.Persistence.Configurations;

public sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.PersonnelNumber).HasMaxLength(50).IsRequired();
        builder.Property(entity => entity.NormalizedPersonnelNumber).HasMaxLength(50).IsRequired();
        builder.Property(entity => entity.FatherName).HasMaxLength(100);
        builder.Property(entity => entity.BirthCertificateNumber).HasMaxLength(50);
        builder.Property(entity => entity.BirthCertificateIssuePlace).HasMaxLength(150);
        builder.Property(entity => entity.MobileNumber).HasMaxLength(20);
        builder.Property(entity => entity.NormalizedMobileNumber).HasMaxLength(20);
        builder.Property(entity => entity.PhoneNumber).HasMaxLength(20);
        builder.Property(entity => entity.Email).HasMaxLength(254);
        builder.Property(entity => entity.EmergencyContactName).HasMaxLength(150);
        builder.Property(entity => entity.EmergencyContactPhone).HasMaxLength(20);
        builder.Property(entity => entity.EmergencyContactRelation).HasMaxLength(50);
        builder.Property(entity => entity.InsuranceNumber).HasMaxLength(50);
        builder.HasOne<Person>().WithOne().HasForeignKey<Employee>(entity => entity.PersonId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ManagedFile>().WithMany().HasForeignKey(entity => entity.ProfilePhotoFileId).OnDelete(DeleteBehavior.SetNull);
        builder.HasIndex(entity => entity.PersonId).IsUnique();
        builder.HasIndex(entity => entity.PersonnelNumber).IsUnique();
        builder.HasIndex(entity => entity.NormalizedPersonnelNumber);
        builder.HasIndex(entity => entity.NormalizedMobileNumber);
        builder.HasIndex(entity => entity.IsDeleted);
    }
}
