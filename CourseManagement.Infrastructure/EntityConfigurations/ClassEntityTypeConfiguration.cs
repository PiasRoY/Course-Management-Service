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
            .HasMaxLength(100)
            .HasColumnType("citext");

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
            .HasMaxLength(50);

        builder
            .HasOne(cl => cl.Instructor)
            .WithMany()
            .HasForeignKey(cl => cl.InstructorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(cl => cl.InstructorId);

        builder
            .HasIndex(cl => cl.Name)
            .IsUnique();
    }
}
