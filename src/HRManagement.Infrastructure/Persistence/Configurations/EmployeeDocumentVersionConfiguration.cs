using HRManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagement.Infrastructure.Persistence.Configurations;

public sealed class EmployeeDocumentVersionConfiguration : IEntityTypeConfiguration<EmployeeDocumentVersion>
{
    public void Configure(EntityTypeBuilder<EmployeeDocumentVersion> builder)
    {
        builder.ToTable("EmployeeDocumentVersions");
        builder.HasKey(entity => entity.Id);
        builder.HasOne<EmployeeDocument>().WithMany().HasForeignKey(entity => entity.EmployeeDocumentId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<ManagedFile>().WithMany().HasForeignKey(entity => entity.ManagedFileId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(entity => new { entity.EmployeeDocumentId, entity.VersionNumber }).IsUnique();
        builder.HasIndex(entity => entity.EmployeeDocumentId).IsUnique().HasFilter("IsCurrent = 1");
    }
}
