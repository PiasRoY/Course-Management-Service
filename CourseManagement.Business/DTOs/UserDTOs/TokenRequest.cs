using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Business.DTOs.UserDTOs;

public class TokenRequest
{
    [Required(ErrorMessage = "Expired access token required.")]
    required public string ExpiredAccessToken { get; set; }

    [Required(ErrorMessage = "Refresh Token is required.")]
    required public string CurrentRefreshToken { get; set; }
}
