using System.Text.Json.Serialization;

namespace CourseManagement.Business.DTOs.EnrollmentDTOs;

public class EnrollmentDto
{
    public required Guid EnrollmentId { get; set; }
    public required Guid StudentId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? StudentEmail { get; set; }
    public required Guid ClassId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ClassName { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Guid? CourseId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CourseName { get; set; }
    public required string EnrolledBy { get; set; }
    public required DateTime EnrolledAt { get; set; }
}
