using CourseManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseManagement.Infrastructure.EntityConfigurations;

public class User_UserRoleEntityTypeConfiguration : IEntityTypeConfiguration<User_UserRole>
{
    public void Configure(EntityTypeBuilder<User_UserRole> builder)
    {
        builder.ToTable("User_UserRole");

        builder.HasKey(uur => new { uur.UserId, uur.RoleId });

        builder
            .HasOne(uur => uur.User)
            .WithMany(u => u.UserUserRoles)
            .HasForeignKey(uur => uur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(uur => uur.UserRole)
            .WithMany()
            .HasForeignKey(uur => uur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
