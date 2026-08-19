using CourseManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseManagement.Infrastructure.EntityConfigurations;

public class UserRoleEntityTypeConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRoles");

        builder.HasKey(ur => ur.RoleId);

        builder
            .Property(ur => ur.RoleName)
            .IsRequired()
            .HasMaxLength(100);
    }
}
