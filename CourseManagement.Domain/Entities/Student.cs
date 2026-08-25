using CourseManagement.Domain.Common;
using CourseManagement.Domain.Enums;

namespace CourseManagement.Domain.Entities;

public class Student : BaseAuditEntity
{
    required public Guid StudentId { get; set; }
    required public Guid UserId { get; set; }
    required public string RollNumber { get; set; }
    required public StudentStatus Status { get; set; }
    required public DateTime AdmissionDate { get; set; }
    public DateTime? GraduationDate { get; set; }
    public double? CGPA { get; set; }
    public double TotalCreditsTaken { get; set; } = 0;
    required public int? CurrentTerm { get; set; }
    required public int? CurrentSemester { get; set; }
    public User User { get; set; } = null!;
    public ICollection<Enrollment> Enrollments = [];
}