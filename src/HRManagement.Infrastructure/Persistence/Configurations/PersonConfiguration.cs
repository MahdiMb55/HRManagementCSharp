using HRManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagement.Infrastructure.Persistence.Configurations;

public sealed class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("Persons");
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(entity => entity.LastName).HasMaxLength(100).IsRequired();
        builder.Property(entity => entity.NormalizedFirstName).HasMaxLength(100).IsRequired();
        builder.Property(entity => entity.NormalizedLastName).HasMaxLength(100).IsRequired();
        builder.Property(entity => entity.NationalCode).HasMaxLength(10).IsRequired();
        builder.Property(entity => entity.BirthDate).HasConversion<NullableDateOnlyTextConverter>().HasColumnType("TEXT");
        builder.HasIndex(entity => entity.NationalCode).IsUnique();
        builder.HasIndex(entity => new { entity.NormalizedLastName, entity.NormalizedFirstName });
    }
}
