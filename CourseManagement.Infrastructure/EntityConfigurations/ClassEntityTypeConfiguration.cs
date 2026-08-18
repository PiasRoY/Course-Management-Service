using CourseManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseManagement.Infrastructure.EntityConfigurations;

public class ClassEntityTypeConfiguration : IEntityTypeConfiguration<Class>
{
    public void Configure(EntityTypeBuilder<Class> builder)
    {
        builder.ToTable("Classes");

        builder.HasKey(cl => cl.ClassId);

        builder
            .Property(cl => cl.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder
            .Property(cl => cl.Semester)
            .IsRequired()
            .HasConversion<string>();

        builder
            .Property(cl => cl.Year)
            .IsRequired();

        builder
            .Property(cl => cl.SectionCode)
            .IsRequired()
            .HasMaxLength(10);

        builder
            .HasOne(cl => cl.Instructor)
            .WithMany()
            .HasForeignKey(cl => cl.InstructorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(cl => cl.Courses)
            .WithMany(c => c.Classes)
            .UsingEntity(
                "CoursesClasses",
                j => {
                    j
                    .HasOne(typeof(Course))
                    .WithMany()
                    .HasForeignKey("CourseId")
                    .OnDelete(DeleteBehavior.Restrict);

                    j
                    .HasOne(typeof(Class))
                    .WithMany()
                    .HasForeignKey("ClassId")
                    .OnDelete(DeleteBehavior.Restrict);
                }
            );

        builder
            .HasIndex(cl => cl.InstructorId);
    }
}
