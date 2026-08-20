using CourseManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseManagement.Infrastructure.EntityConfigurations;

public class StudentEntityTypeConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Students");

        builder.HasKey(s => s.StudentId);

        builder
            .Property(s => s.StudentNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder
            .Property(s => s.Status)
            .IsRequired()
            .HasConversion<string>();

        builder
            .Property(s => s.EnrollmentDate)
            .IsRequired();

        builder
            .Property(s => s.GraduationDate);

        builder
            .Property(s => s.CGPA);

        builder
            .Property(s => s.TotalCreditsEarned)
            .IsRequired();

        builder
            .Property(s => s.CurrentTerm)
            .IsRequired();

        builder
            .Property(s => s.CurrentSemester)
            .IsRequired();

        builder
            .HasOne(s => s.User)
            .WithOne()
            .HasForeignKey<Student>(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasIndex(s => s.StudentNumber)
            .IsUnique();
    }
}
