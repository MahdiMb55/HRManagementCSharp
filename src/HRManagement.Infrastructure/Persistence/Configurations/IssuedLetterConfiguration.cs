using HRManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagement.Infrastructure.Persistence.Configurations;

public sealed class IssuedLetterConfiguration : IEntityTypeConfiguration<IssuedLetter>
{
    public void Configure(EntityTypeBuilder<IssuedLetter> builder)
    {
        builder.ToTable("IssuedLetters");
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.LetterNumber).HasMaxLength(100).IsRequired();
        builder.Property(entity => entity.IssueDate).HasConversion<DateOnlyTextConverter>().HasColumnType("TEXT");
        builder.Property(entity => entity.Subject).HasMaxLength(500);
        builder.HasOne<Employee>().WithMany().HasForeignKey(entity => entity.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<LetterTemplate>().WithMany().HasForeignKey(entity => entity.LetterTemplateId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ManagedFile>().WithMany().HasForeignKey(entity => entity.OutputFileId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(entity => entity.LetterNumber);
        builder.HasIndex(entity => new { entity.EmployeeId, entity.IssueDate });
    }
}
