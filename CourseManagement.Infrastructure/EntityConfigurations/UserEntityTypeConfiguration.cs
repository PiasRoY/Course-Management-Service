using CourseManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseManagement.Infrastructure.EntityConfigurations;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.UserId);

        builder
            .Property(u => u.EmailAddress)
            
            .HasMaxLength(100)
            .IsRequired();

        builder
            .Property(u => u.PasswordHash)
            .HasMaxLength(int.MaxValue)
            .IsRequired();

        builder
            .Property(u => u.FirstName)
            .HasMaxLength(50)
            .IsRequired();

        builder
            .Property(u => u.LastName)
            .HasMaxLength(50)
            .IsRequired();

        builder
            .Property(u => u.Role)
            .IsRequired()
            .HasConversion<string>();

        builder
            .HasIndex(u => u.EmailAddress)
            .IsUnique();
    }
}
