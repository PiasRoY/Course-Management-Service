using CourseManagement.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace CourseManagement.Infrastructure.ApplicationData;

public class ApplicationDbContext : DbContext
{
    public const string DefaultSchema = "course.managment";

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(DefaultSchema);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        ConfigureAuditProperties(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    private static void ConfigureAuditProperties(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseAuditEntity).IsAssignableFrom(entityType.ClrType))
            {
                var builder = modelBuilder.Entity(entityType.ClrType);

                builder.Property(nameof(BaseAuditEntity.CreatedBy));
                builder.Property(nameof(BaseAuditEntity.CreatedAt));
                builder.Property(nameof(BaseAuditEntity.UpdatedBy));
                builder.Property(nameof(BaseAuditEntity.UpdatedAt));
            }
        }
    }
}
