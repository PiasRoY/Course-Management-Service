using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Business.Options;

public class AuthOptions
{
    [Required]
    required public string Issuer { get; set; }
    [Required]
    required public string Audience { get; set; }
    [Required, MinLength(32)]
    required public string Secret { get; set; }
    [Required]
    required public int AccessTokenExpireInSeconds { get; set; }
    [Required]
    required public int RefreshTokenExpireInMinutes { get; set; }
    [Required]
    required public IEnumerable<string> ValidAlgorithms { get; set; }
}
