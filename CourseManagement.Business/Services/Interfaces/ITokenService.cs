using CourseManagement.Business.DTOs.UserDTOs;
using CourseManagement.Domain.Entities;
using System.Security.Claims;

namespace CourseManagement.Business.Services.Interfaces;

public interface ITokenService
{
    Task<TokenDto> GenerateTokensAsync(IEnumerable<Claim> claims, CancellationToken cancellationToken, string? previousRefreshToken = null);
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
    Task<ClaimsPrincipal> ExtractClaimsPrincipalFromTokenAsync(string jwtToken, CancellationToken cancellationToken);
    Task<DateTime> SaveRefreshTokenInfoAsync(string refreshToken, Guid userId, CancellationToken cancellationToken, string? previousRefreshToken = null);
    Task RevokeAllRefreshTokensByUserAsync(Guid userId, CancellationToken cancellationToken);
}
