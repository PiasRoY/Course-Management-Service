using CourseManagement.Domain.Entities;
using CourseManagement.Domain.Enums;

namespace CourseManagement.Business.DTOs.StudentsDTOs;

public class StudentDto
{
    required public Guid StudentId { get; set; }
    required public string StudentNumber { get; set; }
    required public StudentStatus Status { get; set; }
    required public DateTime AdmissionDate { get; set; }
    public DateTime? GraduationDate { get; set; }
    public double? CGPA { get; set; }
    public double TotalCreditsEarned { get; set; } = 0.00;
    required public int CurrentTerm { get; set; }
    required public int CurrentSemester { get; set; }
    public User User { get; set; } = null!;
}
