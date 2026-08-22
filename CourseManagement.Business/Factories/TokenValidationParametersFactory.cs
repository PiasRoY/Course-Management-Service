using CourseManagement.Business.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CourseManagement.Business.Factories;

public class TokenValidationParametersFactory
{
    public static TokenValidationParameters Create(AuthOptions authOptions)
    {
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = authOptions.Issuer,
            ValidAudience = authOptions.Audience,
            ValidAlgorithms = authOptions.ValidAlgorithms,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authOptions.Secret))
        };
    }
}
