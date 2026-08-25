using CourseManagement.Business.DTOs.StudentsDTOs;
using CourseManagement.Domain.Entities;
using CourseManagement.Domain.Enums;

namespace CourseManagement.Business.Mappers;

public static class StudentMapper
{
    public static StudentDto MapsToStudentDto(Student student)
    {
        return new StudentDto
        {
            StudentId = student.StudentId,
            StudentNumber = student.RollNumber,
            Status = student.Status,
            AdmissionDate = student.AdmissionDate,
            GraduationDate = student.GraduationDate,
            CGPA = student.CGPA,
            TotalCreditsEarned = student.TotalCreditsEarned,
            CurrentTerm = student.CurrentTerm,
            CurrentSemester = student.CurrentSemester
        };
    }

    public static Student MapsToStudent(CreateStudentRequest createStudentRequest, Guid userId)
    {
        var student = new Student
        {
            StudentId = Guid.NewGuid(),
            UserId = userId,
            RollNumber = createStudentRequest.RollNumber,
            Status = createStudentRequest.Status,
            AdmissionDate = DateTime.SpecifyKind(createStudentRequest.AdmissionDate, DateTimeKind.Utc),
            CurrentTerm = createStudentRequest.CurrentTerm,
            CurrentSemester = createStudentRequest.CurrentSemester
        };

        if (createStudentRequest.GraduationDate != null)
        {
            student.GraduationDate = DateTime.SpecifyKind(createStudentRequest.GraduationDate!.Value, DateTimeKind.Utc);
        }

        return student;
    }

    public static void UpdateStudent(Student student, UpdateStudentRequest updateStudentRequest)
    {
        student.RollNumber = updateStudentRequest.RollNumber ?? student.RollNumber;
        student.Status = updateStudentRequest.Status ?? student.Status;
        student.CurrentTerm = updateStudentRequest.CurrentTerm ?? student.CurrentTerm;
        student.CurrentSemester = updateStudentRequest.CurrentSemester ?? student.CurrentSemester;

        if (updateStudentRequest.AdmissionDate != null)
        {
            student.AdmissionDate = DateTime.SpecifyKind(updateStudentRequest.AdmissionDate!.Value, DateTimeKind.Utc);
        }

        if (updateStudentRequest.GraduationDate != null)
        {
            student.GraduationDate = DateTime.SpecifyKind(updateStudentRequest.GraduationDate!.Value, DateTimeKind.Utc);
        }
    }
}
