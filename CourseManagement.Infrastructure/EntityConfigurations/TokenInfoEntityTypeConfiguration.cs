using CourseManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseManagement.Infrastructure.EntityConfigurations;

public class TokenInfoEntityTypeConfiguration : IEntityTypeConfiguration<TokenInfo>
{
    public void Configure(EntityTypeBuilder<TokenInfo> builder)
    {
        builder.ToTable("TokenInfos");

        builder.HasKey(t => t.TokenId);

        builder
            .Property(t => t.UserId)
            .IsRequired();

        builder
            .Property(t => t.TokenHash)
            .IsRequired();

        builder
            .Property(t => t.ExpiresAt)
            .IsRequired();

        builder
            .Property(t => t.RevokedAt);

        builder
            .Property(t => t.ReplacedByTokenId);

        builder
            .HasIndex(t => t.UserId);

        builder
            .HasIndex(t => t.TokenHash);
    }
}
