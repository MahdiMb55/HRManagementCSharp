using HRManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagement.Infrastructure.Persistence.Configurations;

public sealed class EmployeeDocumentConfiguration : IEntityTypeConfiguration<EmployeeDocument>
{
    public void Configure(EntityTypeBuilder<EmployeeDocument> builder)
    {
        builder.ToTable("EmployeeDocuments");
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.Title).HasMaxLength(200).IsRequired();
        builder.HasOne<Employee>().WithMany().HasForeignKey(entity => entity.EmployeeId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<DocumentCategory>().WithMany().HasForeignKey(entity => entity.CategoryId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(entity => new { entity.EmployeeId, entity.CategoryId, entity.IsDeleted });
    }
}
