using CourseManagement.Business.DTOs.BulkImportDTOs;
using CourseManagement.Business.DTOs.ClassDTOs;
using CourseManagement.Business.DTOs.CourseDTOs;
using CourseManagement.Business.DTOs.EnrollmentDTOs;
using CourseManagement.Business.DTOs.StudentsDTOs;
using CourseManagement.Business.DTOs.UserDTOs;
using CourseManagement.Business.Enums;
using CourseManagement.Business.Services.Interfaces;
using CourseManagement.Domain.Common;
using CourseManagement.Domain.Entities;
using CourseManagement.Domain.Enums;
using Hangfire;
using Hangfire.Storage;
using System.Security.Claims;

namespace CourseManagement.Business.Services;

public class TaskManager : ITaskManager
{
    private readonly IMonitoringApi monitoringApi;
    private readonly IBulkService bulkService;
    private readonly IAuthService authService;
    private readonly IClassService classService;
    private readonly ICourseService courseService;
    private readonly IStudentService studentService;
    private readonly IEnrollmentService enrollmentService;
    private readonly ICurrentUserContext currentUserContext;

    public TaskManager(
        JobStorage jobStorage,
        IBulkService bulkService,
        IAuthService authService,
        IClassService classService,
        ICourseService courseService,
        IStudentService studentService,
        IEnrollmentService enrollmentService,
        ICurrentUserContext currentUserContext)
    {
        this.monitoringApi = jobStorage.GetMonitoringApi();
        this.bulkService = bulkService;
        this.authService = authService;
        this.classService = classService;
        this.courseService = courseService;
        this.studentService = studentService;
        this.enrollmentService = enrollmentService;
        this.currentUserContext = currentUserContext;
    }

    public string? JobStatus(string jobId)
    {
        return monitoringApi.JobDetails(jobId).History.FirstOrDefault()?.StateName;
    }

    public string EnqueueBulkImportJob(UserContextDto userContextDto, JobEvent jobEvent, ImportTypes importTypes)
    {
        return BackgroundJob.Enqueue<ITaskManager>(taskService => taskService.BulkImportAsync(userContextDto, jobEvent, importTypes));
    }

    public async Task BulkImportAsync(UserContextDto userContextDto, JobEvent jobEvent, ImportTypes importTypes)
    {
        this.currentUserContext.SetUserContext(userContextDto.UserId, userContextDto.UserEmail, userContextDto.Roles);

        switch (importTypes)
        {
            case ImportTypes.Users:
                await this.bulkService.ProcessBulkImportAsync<CreateUserRequest, UserDto>(
                    jobEvent,
                    (req, ct) => this.authService.CreateUserAsync(req, ct),
                    CancellationToken.None);
                break;

            case ImportTypes.Classes:
                await this.bulkService.ProcessBulkImportAsync<CreateClassRequest, ClassDto>(
                    jobEvent,
                    (req, ct) => this.classService.CreateClassAsync(req, ct),
                    CancellationToken.None);
                break;

            case ImportTypes.Courses:
                await this.bulkService.ProcessBulkImportAsync<CreateCourseRequest, CourseDto>(
                    jobEvent,
                    (req, ct) => this.courseService.CreateCourseAsync(req, ct),
                    CancellationToken.None);
                break;

            case ImportTypes.Students:
                await this.bulkService.ProcessBulkImportAsync<CreateStudentRequest, StudentDto>(
                    jobEvent,
                    (req, ct) => this.studentService.CreateStudentAsync(req, ct),
                    CancellationToken.None);
                break;

            case ImportTypes.Enrollments:
                await this.bulkService.ProcessBulkImportAsync<CreateEnrollmentByClassNames, EnrollmentDto>(
                    jobEvent,
                    (req, ct) => this.enrollmentService.CreateEnrollmentByClassNamesAsync(req, ct),
                    CancellationToken.None);
                break;
        }
    }
}