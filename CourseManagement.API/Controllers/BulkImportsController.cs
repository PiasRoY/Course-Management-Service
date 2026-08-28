using CourseManagement.Business.DTOs.BulkImportDTOs;
using CourseManagement.Business.Enums;
using CourseManagement.Business.Services;
using CourseManagement.Business.Services.Interfaces;
using CourseManagement.Domain.Enums;
using Hangfire.States;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace CourseManagement.API.Controllers
{
    [Authorize(Roles = $"{nameof(UserRoles.Admin)}, {nameof(UserRoles.Staff)}")]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BulkImportsController : ControllerBase
    {
        private readonly IJobEventService jobEventService;
        private readonly IBulkService bulkService;
        private readonly ITaskManager taskService;

        public BulkImportsController(
            IJobEventService jobEventService,
            IBulkService bulkService, 
            ITaskManager taskService)
        {
            this.jobEventService = jobEventService;
            this.bulkService = bulkService;
            this.taskService = taskService;
        }

        [HttpPost("{importType}")]
        public async Task<ActionResult> ImportUsersAsync(ImportTypes importType, IFormFile file, CancellationToken cancellationToken)
        {
            ActionResult? result = this.ValidateFile(file);
            if (result != null)
            {
                return result;
            }

            var jobEvent = await this.bulkService.PreprocessingAsync(file, cancellationToken);
            var jobId = this.taskService.EnqueueBulkImportUsersJob(jobEvent, importType);
            await this.bulkService.PostProcessingAsync(jobEvent, jobId, cancellationToken);

            return AcceptedAtAction(
                nameof(GetStatus),
                new { jobEventId = jobEvent.JobEventId }, 
                new { jobEventId = jobEvent.JobEventId });
        }

        [HttpGet("status/{jobEventId}")]
        public async Task<StatusResult> GetStatus([FromRoute] Guid jobEventId, CancellationToken cancellationToken)
        {
            var jobEvent = await this.jobEventService.GetJobEventByAsync(jobEventId, cancellationToken);

            if (jobEvent.JobEventStatus == BulkProcessStatus.Failed)
            {
                return new StatusResult
                {
                    Status = jobEvent.JobEventStatus,
                    DownloadUrl = this.Url.Action(nameof(DownloadOutputFile), new { jobEventId })
                };
            }

            return new StatusResult { Status = jobEvent.JobEventStatus };
        }

        [HttpGet("download/{jobEventId}")]
        public async Task<ActionResult> DownloadOutputFile([FromRoute] Guid jobEventId, CancellationToken cancellationToken)
        {
            var jobEvent = await this.jobEventService.GetJobEventByAsync(jobEventId, cancellationToken);
            var fileStream = await this.bulkService.DownloadOutputCsvFileAsync(jobEvent, cancellationToken);
            return File(fileStream, MediaTypeNames.Text.Csv, fileDownloadName: $"OutputFile_{jobEventId}");
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
