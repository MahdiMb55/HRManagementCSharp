using HRManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagement.Infrastructure.Persistence.Configurations;

public sealed class LetterTemplateConfiguration : IEntityTypeConfiguration<LetterTemplate>
{
    public void Configure(EntityTypeBuilder<LetterTemplate> builder)
    {
        builder.ToTable("LetterTemplates");
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.Title).HasMaxLength(200).IsRequired();
        builder.HasOne<ManagedFile>().WithMany().HasForeignKey(entity => entity.ManagedFileId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(entity => entity.IsActive);
    }
}
