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
        return new Student
        {
            StudentId = Guid.NewGuid(),
            UserId = userId,
            RollNumber = createStudentRequest.RollNumber,
            Status = createStudentRequest.Status,
            AdmissionDate = createStudentRequest.AdmissionDate,
            GraduationDate = createStudentRequest.GraduationDate ?? null,
            CurrentTerm = createStudentRequest.CurrentTerm,
            CurrentSemester = createStudentRequest.CurrentSemester
        };
    }

    public static void UpdateStudent(Student student, UpdateStudentRequest updateStudentRequest)
    {
        student.RollNumber = updateStudentRequest.RollNumber ?? student.RollNumber;
        student.Status = updateStudentRequest.Status ?? student.Status;
        student.AdmissionDate = updateStudentRequest.AdmissionDate ?? student.AdmissionDate;
        student.GraduationDate = updateStudentRequest.GraduationDate ?? student.GraduationDate;
        student.CurrentTerm = updateStudentRequest.CurrentTerm ?? student.CurrentTerm;
        student.CurrentSemester = updateStudentRequest.CurrentSemester ?? student.CurrentSemester;
    }
}
