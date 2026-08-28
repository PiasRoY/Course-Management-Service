using CourseManagement.Business.DTOs.ClassDTOs;
using CourseManagement.Business.DTOs.CourseDTOs;
using CourseManagement.Business.DTOs.EnrollmentDTOs;
using CourseManagement.Business.DTOs.StudentsDTOs;
using CourseManagement.Business.DTOs.UserDTOs;
using CourseManagement.Business.Enums;
using CourseManagement.Business.Services.Interfaces;
using CourseManagement.Domain.Entities;
using Hangfire;
using Hangfire.Storage;

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

    public TaskManager(
        JobStorage jobStorage,
        IBulkService bulkService,
        IAuthService authService,
        IClassService classService,
        ICourseService courseService,
        IStudentService studentService,
        IEnrollmentService enrollmentService)
    {
        this.monitoringApi = jobStorage.GetMonitoringApi();
        this.bulkService = bulkService;
        this.authService = authService;
        this.classService = classService;
        this.courseService = courseService;
        this.studentService = studentService;
        this.enrollmentService = enrollmentService;
    }

    public string? JobStatus(string jobId)
    {
        return monitoringApi.JobDetails(jobId).History.FirstOrDefault()?.StateName;
    }

    public string EnqueueBulkImportUsersJob(JobEvent jobEvent, ImportTypes importTypes)
    {
        return BackgroundJob.Enqueue<ITaskManager>(taskService => taskService.BulkImportAsync(jobEvent, importTypes));
    }

    public async Task BulkImportAsync(JobEvent jobEvent, ImportTypes importTypes)
    {
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
                await this.bulkService.ProcessBulkImportAsync<CreateEnrollmentRequest, EnrollmentDto>(
                    jobEvent,
                    (req, ct) => this.enrollmentService.CreateEnrollmentAsync(req, ct),
                    CancellationToken.None);
                break;
        }
    }
}