using CourseManagement.Domain.Entities;
using CourseManagement.Infrastructure.ApplicationData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseManagement.Infrastructure.EntityConfigurations;

public class CourseEntityTypeConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("Courses", t =>
        {
            t.HasCheckConstraint(
                "Check_Courses_Name_Alphanumeric",
                $"\"Name\" ~ '{DbConstants.AlphaNumericRegex}'");
        });

        builder.HasKey(c => c.CourseId);

        builder
            .Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnType("citext");

        builder
            .Property(c => c.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .Property(c => c.Credits)
            .IsRequired();

        builder
            .HasIndex(c => c.Name)
            .IsUnique();
    }
}
