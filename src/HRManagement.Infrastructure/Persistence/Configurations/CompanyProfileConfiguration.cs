using HRManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagement.Infrastructure.Persistence.Configurations;

public sealed class CompanyProfileConfiguration : IEntityTypeConfiguration<CompanyProfile>
{
    public void Configure(EntityTypeBuilder<CompanyProfile> builder)
    {
        builder.ToTable("CompanyProfiles", table => table.HasCheckConstraint("CK_CompanyProfiles_Singleton", "Id = 1"));
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.CompanyName).HasMaxLength(200).IsRequired();
        builder.Property(entity => entity.NationalIdentifier).HasMaxLength(20);
        builder.Property(entity => entity.RegistrationNumber).HasMaxLength(50);
        builder.Property(entity => entity.PhoneNumber).HasMaxLength(20);
        builder.HasOne<ManagedFile>().WithMany().HasForeignKey(entity => entity.LogoFileId).OnDelete(DeleteBehavior.SetNull);
    }
}
