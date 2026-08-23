using CourseManagement.Business.DTOs.UserDTOs;
using CourseManagement.Business.Factories;
using CourseManagement.Business.Options;
using CourseManagement.Business.Services.Interfaces;
using CourseManagement.Domain.Entities;
using CourseManagement.Infrastructure.ApplicationData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CourseManagement.Business.Services;

public class TokenService : ITokenService
{
    private readonly ApplicationDbContext dbContext;
    private readonly AuthOptions authOptions;

    public TokenService(
        ApplicationDbContext applicationDbContext,
        IOptions<AuthOptions> authOptions)
    {
        this.dbContext = applicationDbContext;
        this.authOptions = authOptions.Value;
    }

    public async Task<TokenDto> GenerateTokensAsync(IEnumerable<Claim> claims, string? previousRefreshToken = null)
    {
        var userIdStr = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        
        if(!Guid.TryParse(userIdStr, out var userId))
        {
            throw new InvalidOperationException("Claim UserId is invalid.");
        }

        var accessToken = this.GenerateAccessToken(claims);
        var newRefreshToken = this.GenerateRefreshToken();
        var expiredAt = await this.SaveRefreshTokenInfoAsync(newRefreshToken, userId, previousRefreshToken);

        return new TokenDto
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken,
            RereshTokenExpiredAt = expiredAt
        };
    }

    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var secretKey = Encoding.UTF8.GetBytes(this.authOptions.Secret);
        var algorithm = authOptions.ValidAlgorithms.FirstOrDefault();

        if (string.IsNullOrEmpty(algorithm))
        {
            throw new InvalidOperationException("No valid algorithm signature is configured.");
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddSeconds(this.authOptions.AccessTokenExpireInSeconds),
            Issuer = this.authOptions.Issuer,
            Audience = this.authOptions.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), algorithm)
        };

        var tokenHandler = new JsonWebTokenHandler();
        return tokenHandler.CreateToken(tokenDescriptor);
    }

    public string GenerateRefreshToken()
    {
        var randomNumberBytes = new Byte[256];

        RandomNumberGenerator.Create().GetBytes(randomNumberBytes);

        return Convert.ToBase64String(randomNumberBytes);
    }

    public async Task<ClaimsPrincipal> ExtractClaimsPrincipalFromTokenAsync(string jwtToken)
    {
        var tokenValidationParams = TokenValidationParametersFactory.Create(authOptions);
        var tokenHandler = new JsonWebTokenHandler();

        tokenValidationParams.ValidateLifetime = false;

        var result = await tokenHandler.ValidateTokenAsync(jwtToken, tokenValidationParams);

        if (!result.IsValid)
        {
            throw new SecurityTokenException("Invalid token.", result.Exception);
        }

        return new ClaimsPrincipal(result.ClaimsIdentity);
    }

    public async Task<DateTime> SaveRefreshTokenInfoAsync(string refreshToken, Guid userId, string? previousRefreshToken = null)
    {
        var tokenInfo = new TokenInfo
        {
            TokenId = Guid.NewGuid(),
            UserId = userId,
            TokenHash = this.HashConversion(refreshToken),
            ExpiresAt = DateTime.UtcNow.AddMinutes(this.authOptions.RefreshTokenExpireInMinutes)
        };

        if (!string.IsNullOrEmpty(previousRefreshToken))
        {
            await ReplaceOldRefreshToken(previousRefreshToken, tokenInfo);
        }

        await this.dbContext.TokenInfos.AddAsync(tokenInfo);
        await this.dbContext.SaveChangesAsync();

        return tokenInfo.ExpiresAt;
    }

    public async Task RevokeAllRefreshTokensByUser(Guid userId)
    {
        var tokens = await this.dbContext.TokenInfos.Where(t => t.UserId == userId).ToListAsync();

        foreach (var token in tokens)
        {
            token.RevokedAt = DateTime.UtcNow;
        }

        await this.dbContext.SaveChangesAsync();
    }

    private async Task ReplaceOldRefreshToken(string previousRefreshToken, TokenInfo tokenInfo)
    {
        var tokenHash = this.HashConversion(previousRefreshToken);
        var oldTokenInfo = await this.dbContext
            .TokenInfos
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash && t.UserId == tokenInfo.UserId);

        if (oldTokenInfo == null)
        {
            throw new InvalidOperationException("Invalid previous refresh token.");
        }

        if (DateTime.UtcNow > oldTokenInfo.ExpiresAt)
        {
            throw new InvalidOperationException("Refresh token has been expired.");
        }

        if (oldTokenInfo.RevokedAt != null)
        {
            throw new InvalidOperationException("Refresh token is invalid.");
        }

        oldTokenInfo.RevokedAt = DateTime.UtcNow;
        oldTokenInfo.ReplacedByTokenId = tokenInfo.TokenId;
    }

    private string HashConversion(string token)
    {
        var tokenBytes = Encoding.UTF8.GetBytes(token);
        return Convert.ToBase64String(SHA256.HashData(tokenBytes));
    }
}
