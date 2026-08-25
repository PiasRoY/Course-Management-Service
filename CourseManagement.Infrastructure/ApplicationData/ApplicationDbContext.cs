using CourseManagement.Domain.Common;
using CourseManagement.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Security.Claims;

namespace CourseManagement.Infrastructure.ApplicationData;

public class ApplicationDbContext : DbContext
{
    public const string DefaultSchema = "course.managment";
    private readonly IHttpContextAccessor httpContextAccessor;

    public DbSet<User> Users { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Class> Classes { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<TokenInfo> TokenInfos { get; set; }

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IHttpContextAccessor httpContextAccessor) : base(options) 
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    private string CurrentUser => 
        this.httpContextAccessor.HttpContext?.User?.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value
        ?? "11111111-1111-1111-1111-111111111111"; // SYSTEM ID

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(DefaultSchema);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        ConfigureAuditProperties(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SaveAuditProperties();
        return base.SaveChangesAsync(cancellationToken);
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

    private void SaveAuditProperties()
    {
        var entries = ChangeTracker.Entries<BaseAuditEntity>();

        foreach(var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = Guid.Parse(CurrentUser);
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedBy = Guid.Parse(CurrentUser);
                    break;
            }
        }
    }
}
