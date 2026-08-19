using CourseManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseManagement.Infrastructure.EntityConfigurations;

public class CoursesClassesEntityTypeConfiguration : IEntityTypeConfiguration<CourseClass>
{
    public void Configure(EntityTypeBuilder<CourseClass> builder)
    {
        builder.ToTable("CoursesClasses");

        builder.HasKey(cc => new { cc.CourseId, cc.ClassId });

        builder
            .HasOne(cc => cc.Course)
            .WithMany(c => c.CourseClasses)
            .HasForeignKey(cc => cc.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(cc => cc.Class)
            .WithMany(cl => cl.CourseClasses)
            .HasForeignKey(cc => cc.ClassId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
