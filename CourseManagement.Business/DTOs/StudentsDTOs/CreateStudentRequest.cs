using CourseManagement.Business.Constants;
using CourseManagement.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Business.DTOs.StudentsDTOs;

public class CreateStudentRequest
{
    [Required(ErrorMessage = "Email address is required.")]
    [RegularExpression(RegexConstants.EmailRegex, ErrorMessage = "Email address is invalid.")]
    required public string EmailAddress { get; set; }

    [Required(ErrorMessage = "Student roll number is required.")]
    [RegularExpression(RegexConstants.StudentRollNumberRegex, ErrorMessage = "Roll number is invalid.")]
    required public string RollNumber { get; set; }

    [Required(ErrorMessage = "Student status is required.")]
    required public StudentStatus Status { get; set; }

    [Required(ErrorMessage = "Student admission date.")]
    required public DateTime AdmissionDate { get; set; }

    public DateTime? GraduationDate { get; set; }

    [Required(ErrorMessage = "Current term is required.")]
    public int? CurrentTerm { get; set; }

    [Required(ErrorMessage = "Current semester is required.")]
    public int? CurrentSemester { get; set; }
}
