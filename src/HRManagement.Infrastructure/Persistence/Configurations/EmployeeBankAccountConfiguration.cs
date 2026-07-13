using HRManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagement.Infrastructure.Persistence.Configurations;

public sealed class EmployeeBankAccountConfiguration : IEntityTypeConfiguration<EmployeeBankAccount>
{
    public void Configure(EntityTypeBuilder<EmployeeBankAccount> builder)
    {
        builder.ToTable("EmployeeBankAccounts");
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.BankName).HasMaxLength(100).IsRequired();
        builder.Property(entity => entity.AccountNumber).HasMaxLength(50).IsRequired();
        builder.Property(entity => entity.CardNumber).HasMaxLength(16);
        builder.Property(entity => entity.Iban).HasMaxLength(26);
        builder.HasOne<Employee>().WithMany().HasForeignKey(entity => entity.EmployeeId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(entity => entity.CardNumber).IsUnique().HasFilter("CardNumber IS NOT NULL");
        builder.HasIndex(entity => entity.Iban).IsUnique().HasFilter("Iban IS NOT NULL");
        builder.HasIndex(entity => entity.EmployeeId).IsUnique().HasFilter("IsActive = 1 AND IsPrimary = 1 AND IsDeleted = 0");
        builder.HasIndex(entity => new { entity.EmployeeId, entity.IsDeleted, entity.IsActive });
    }
}
