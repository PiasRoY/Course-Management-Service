namespace CourseManagement.Business.Constants;

public static class RegexConstants
{
    public const string EmailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
    public const string StudentRollNumberRegex = "^[a-zA-Z0-9]+[._-][a-zA-Z0-9]+$";
}
