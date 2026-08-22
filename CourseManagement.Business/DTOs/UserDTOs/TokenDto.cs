namespace CourseManagement.Business.DTOs.UserDTOs;

public class TokenDto
{
    required public string AccessToken { get; set; }
    required public string RefreshToken { get; set; }
    required public DateTime RereshTokenExpiredAt { get; set; }
}
