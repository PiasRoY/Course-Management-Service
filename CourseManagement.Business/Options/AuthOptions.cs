namespace CourseManagement.Business.Options;

public class AuthOptions
{
    required public string Issuer { get; set; }
    required public string Audience { get; set; }
    required public string Secret { get; set; }
    required public int AccessTokenExpireInSeconds { get; set; }
    required public int RefreshTokenExpireInMinutes { get; set; }
    required public IEnumerable<string> ValidAlgorithms { get; set; }
}
