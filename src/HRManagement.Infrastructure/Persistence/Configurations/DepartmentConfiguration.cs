using HRManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagement.Infrastructure.Persistence.Configurations;

public sealed class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Departments");
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.Name).HasMaxLength(150).IsRequired();
        builder.Property(entity => entity.NormalizedName).HasMaxLength(150).IsRequired();
        builder.HasOne<Department>().WithMany().HasForeignKey(entity => entity.ParentDepartmentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(entity => entity.NormalizedName);
        builder.HasIndex(entity => entity.IsActive);
    }
}
