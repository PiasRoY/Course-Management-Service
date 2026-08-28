using CourseManagement.Business.Constants;
using CourseManagement.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Business.DTOs.StudentsDTOs;

public class UpdateStudentRequest
{
    [RegularExpression(RegexConstants.StudentRollNumberRegex, ErrorMessage = "RollNumber is invalid.")]
    public string? RollNumber { get; set; }
    public StudentStatus? Status { get; set; }
    public DateTime? AdmissionDate { get; set; }
    public DateTime? GraduationDate { get; set; }
    public int? CurrentTerm { get; set; }
    public int? CurrentSemester { get; set; }
}