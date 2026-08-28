using CourseManagement.Domain.Enums;
using System.Text.Json.Serialization;

namespace CourseManagement.Business.DTOs.StudentsDTOs;

public class StudentDto
{
    required public Guid StudentId { get; set; }
    required public string Email { get; set; }
    required public string FullName { get; set; }
    required public string StudentNumber { get; set; }
    required public StudentStatus Status { get; set; }
    required public DateTime AdmissionDate { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? GraduationDate { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    required public int? CurrentTerm { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    required public int? CurrentSemester { get; set; }
}
