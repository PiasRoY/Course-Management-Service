using CourseManagement.Domain.Common;

namespace CourseManagement.Domain.Entities;

public class TokenInfo : BaseAuditEntity
{
    public Guid TokenId { get; set; }
    public Guid UserId { get; set; }
    public required string TokenHash { get; set; }
    public required DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public Guid? ReplacedByTokenId { get; set; }
}
