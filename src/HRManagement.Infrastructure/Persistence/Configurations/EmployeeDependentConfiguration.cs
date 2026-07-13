using HRManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagement.Infrastructure.Persistence.Configurations;

public sealed class EmployeeDependentConfiguration : IEntityTypeConfiguration<EmployeeDependent>
{
    public void Configure(EntityTypeBuilder<EmployeeDependent> builder)
    {
        builder.ToTable("EmployeeDependents");
        builder.HasKey(entity => entity.Id);
        builder.HasOne<Employee>().WithMany().HasForeignKey(entity => entity.EmployeeId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Person>().WithMany().HasForeignKey(entity => entity.DependentPersonId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(entity => new { entity.EmployeeId, entity.DependentPersonId, entity.RelationshipType })
            .IsUnique()
            .HasFilter("IsDeleted = 0");
        builder.HasIndex(entity => entity.IsDeleted);
    }
}
