using HRManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagement.Infrastructure.Persistence.Configurations;

public sealed class ContractAttachmentConfiguration : IEntityTypeConfiguration<ContractAttachment>
{
    public void Configure(EntityTypeBuilder<ContractAttachment> builder)
    {
        builder.ToTable("ContractAttachments");
        builder.HasKey(entity => entity.Id);
        builder.HasOne<Contract>().WithMany().HasForeignKey(entity => entity.ContractId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<ManagedFile>().WithMany().HasForeignKey(entity => entity.ManagedFileId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(entity => new { entity.ContractId, entity.IsDeleted });
    }
}
