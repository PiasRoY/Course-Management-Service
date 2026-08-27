using CourseManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseManagement.Infrastructure.EntityConfigurations;

public class JobEventEntityTypeConfiguration : IEntityTypeConfiguration<JobEvent>
{
    public void Configure(EntityTypeBuilder<JobEvent> builder)
    {
        builder.ToTable("JobEvents");

        builder.HasKey(j => j.JobEventId);

        builder
            .Property(j => j.HangfireJobId)
            .IsRequired();

        builder
            .Property(j => j.JobEventStatus)
            .HasConversion<string>();

        builder
            .Property(j => j.InputFilePath)
            .IsRequired();

        builder
            .Property(j => j.OutputFilePath)
            .IsRequired();
    }
}
