using CourseManagement.Business.DTOs.UserDTOs;
using CourseManagement.Domain.Entities;
using System.Security.Claims;

namespace CourseManagement.Business.Services.Interfaces;

public interface ITokenService
{
    Task<TokenDto> GenerateTokensAsync(IEnumerable<Claim> claims, string? previousRefreshToken = null);
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
    Task<ClaimsPrincipal> ExtractClaimsPrincipalFromTokenAsync(string jwtToken);
    Task<DateTime> SaveRefreshTokenInfoAsync(string refreshToken, Guid userId, string? previousRefreshToken = null);
    Task RevokeAllRefreshTokensByUser(Guid userId);
}
