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
            .Property(s => s.RollNumber)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnType("citext");

        builder
            .Property(s => s.Status)
            .IsRequired()
            .HasConversion<string>();

        builder
            .Property(s => s.AdmissionDate)
            .IsRequired();

        builder
            .Property(s => s.GraduationDate);

        builder
            .Property(s => s.CurrentTerm);

        builder
            .Property(s => s.CurrentSemester);

        builder
            .HasOne(s => s.User)
            .WithOne()
            .HasForeignKey<Student>(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasIndex(s => s.RollNumber)
            .IsUnique();
    }
}
