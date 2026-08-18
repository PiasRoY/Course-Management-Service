using CourseManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseManagement.Infrastructure.EntityConfigurations;

public class CourseEntityTypeConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("Courses");

        builder.HasKey(c => c.CourseId);

        builder
            .Property(c => c.Code)
            .IsRequired()
            .HasMaxLength(10);

        builder
            .Property(c => c.Title)
            .IsRequired()
            .HasMaxLength(50);

        builder
            .Property(c => c.Credits)
            .IsRequired();
    }
}
