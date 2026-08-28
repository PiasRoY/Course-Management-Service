using CourseManagement.Business.Factories;
using CourseManagement.Business.Options;
using CourseManagement.Business.Services;
using CourseManagement.Domain.Entities;
using CourseManagement.Infrastructure.ApplicationData;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CourseManagement.UnitTests.Services.Tests;

public class TokenServiceTests : IDisposable
{
    private readonly ApplicationDbContext dbContext;
    private readonly TokenService tokenService;
    private readonly AuthOptions authOptions;

    public TokenServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        
        this.dbContext = new ApplicationDbContext(options, new CurrentUserContext());
        
        this.authOptions = new AuthOptions
        {
            Issuer = "CourseManagement.AUTH",
            Audience = "CourseManagement.API",
            Secret = "super-secret-making-it-more-big-more-bigger-more-bigger",
            AccessTokenExpireInSeconds = 900,
            RefreshTokenExpireInMinutes = 21600,
            ValidAlgorithms = ["HS256"]
        };

        this.tokenService = new TokenService(this.dbContext, Options.Create(this.authOptions));
    }

    public void Dispose()
    {
        this.dbContext.Dispose();
    }

    [Fact]
    public async Task GenerateTokensAsync_WithValidClaims_ReturnsTokensAndStoresRefreshToken()
    {
        var userId = Guid.NewGuid();
        var result = await this.tokenService.GenerateTokensAsync(
            [new Claim(ClaimTypes.NameIdentifier, userId.ToString())],
            CancellationToken.None);

        Assert.False(string.IsNullOrWhiteSpace(result.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(result.RefreshToken));
        Assert.Single(await this.dbContext.TokenInfos.ToListAsync());
    }

    [Fact]
    public async Task GenerateAccessToken_WithValidClaims_ReturnsJwtAsync()
    {
        var token = this.tokenService.GenerateAccessToken(
            [new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())]);

        Assert.False(string.IsNullOrWhiteSpace(token));

        var tokenValidationParams = TokenValidationParametersFactory.Create(this.authOptions);

        var handler = new JwtSecurityTokenHandler();
        var result = await handler.ValidateTokenAsync(token, tokenValidationParams);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ExtractClaimsPrincipalFromTokenAsync_WithValidToken_ReturnsClaims()
    {
        var userId = Guid.NewGuid();
        var token = this.tokenService.GenerateAccessToken(
            [new Claim(ClaimTypes.NameIdentifier, userId.ToString())]);

        var principal = await this.tokenService.ExtractClaimsPrincipalFromTokenAsync(token, CancellationToken.None);

        Assert.Equal(userId.ToString(), principal.FindFirstValue(ClaimTypes.NameIdentifier));
    }

    [Fact]
    public async Task RevokeAllRefreshTokensByUserAsync_WithExistingTokens_RevokesUserTokens()
    {
        var userId = Guid.NewGuid();
        this.dbContext.TokenInfos.AddRange(
            new TokenInfo
            {
                TokenId = Guid.NewGuid(),
                UserId = userId,
                TokenHash = "token-hash-1",
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            },
            new TokenInfo
            {
                TokenId = Guid.NewGuid(),
                UserId = userId,
                TokenHash = "token-hash-2",
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            });
        await this.dbContext.SaveChangesAsync();

        await this.tokenService.RevokeAllRefreshTokensByUserAsync(userId, CancellationToken.None);

        var tokenInfos = await this.dbContext.TokenInfos.ToListAsync();

        foreach (var tokenInfo in tokenInfos)
        {
            Assert.NotNull(tokenInfo.RevokedAt);
        }
    }
}
