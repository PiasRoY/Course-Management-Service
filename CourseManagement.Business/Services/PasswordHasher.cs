using CourseManagement.Business.Services.Interfaces;
using BCryptNet = BCrypt.Net;

namespace CourseManagement.Business.Services;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        return BCryptNet.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        return BCryptNet.BCrypt.Verify(password, hashedPassword);
    }
}
