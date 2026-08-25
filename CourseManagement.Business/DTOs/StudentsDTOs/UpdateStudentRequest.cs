using CourseManagement.Domain.Enums;

namespace CourseManagement.Business.DTOs.StudentsDTOs;

public class UpdateStudentRequest
{
    public string? RollNumber { get; set; }
    public StudentStatus? Status { get; set; }
    public DateTime? AdmissionDate { get; set; }
    public DateTime? GraduationDate { get; set; }
    public int? CurrentTerm { get; set; }
    public int? CurrentSemester { get; set; }
}