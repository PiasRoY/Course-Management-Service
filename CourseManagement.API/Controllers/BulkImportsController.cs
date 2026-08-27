using CourseManagement.Business.DTOs.BulkImportDTOs;
using CourseManagement.Business.Enums;
using CourseManagement.Business.Services;
using CourseManagement.Business.Services.Interfaces;
using CourseManagement.Domain.Enums;
using Hangfire.States;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagement.API.Controllers
{
    [Authorize(Roles = $"{nameof(UserRoles.Admin)}, {nameof(UserRoles.Staff)}")]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BulkImportsController : ControllerBase
    {
        private readonly IStorageService storageService;
        private readonly IBulkService bulkService;
        private readonly ITaskService taskService;

        public BulkImportsController(
            IStorageService storageService,
            IBulkService bulkService, 
            ITaskService taskService)
        {
            this.storageService = storageService;
            this.bulkService = bulkService;
            this.taskService = taskService;
        }

        [HttpPost]
        public async Task<ActionResult> ImportUsersAsync(IFormFile file, CancellationToken cancellationToken)
        {
            ActionResult? result = this.ValidateFile(file);
            if (result != null)
            {
                return result;
            }

            var jobEvent = await this.bulkService.PreprocessingAsync(file, cancellationToken);
            var jobId = this.taskService.EnqueueBulkImportUsersJob(jobEvent, cancellationToken);
            await this.bulkService.PostProcessingAsync(jobEvent, jobId, cancellationToken);

            return AcceptedAtAction(nameof(GetStatusAsync), new { jobId });
        }

        [HttpGet("{requestId}")]
        public async Task<StatusResult> GetStatusAsync([FromRoute] string requestId)
        {
            var status = this.taskService.JobStatus(requestId);

            if (status == SucceededState.StateName || status == FailedState.StateName)
            {
                return new StatusResult
                {
                    Status = BulkProcessStatus.Completed,
                    OutputFile = this.storageService.OpenLocalFile($"Output_{requestId}.csv")
                };
            }

            return new StatusResult { Status = BulkProcessStatus.Processing };
        }


        private ActionResult? ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            if (!Path.GetExtension(file.FileName).Equals(".csv", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("File must be a .csv file.");
            }

            return null;
        }
    }
}
