using CourseManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseManagement.Infrastructure.EntityConfigurations;

public class EnrollmentEntityTypeConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("Enrollments");

        builder.HasKey(e => e.EnrollmentId);

        builder
            .Property(e => e.EnrolledAt)
            .IsRequired();

        builder
            .HasOne(e => e.Student)
            .WithMany()
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(e => e.Class)
            .WithMany(cl => cl.Enrollments)
            .HasForeignKey(e => e.ClassId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(e => e.EnrolledBy)
            .WithMany()
            .HasForeignKey(e => e.EnrolledById)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(e => e.StudentId);

        builder
            .HasIndex(e => e.ClassId);

        builder
            .HasIndex(e => e.EnrolledById);
    }
}
