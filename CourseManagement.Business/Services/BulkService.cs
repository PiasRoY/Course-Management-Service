using CourseManagement.Business.DTOs.BulkImportDTOs;
using CourseManagement.Business.DTOs.UserDTOs;
using CourseManagement.Business.Services.Helpers;
using CourseManagement.Business.Services.Interfaces;
using CourseManagement.Domain.Entities;
using CourseManagement.Domain.Enums;
using CourseManagement.Infrastructure.ApplicationData;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CourseManagement.Business.Services;

public class BulkService : IBulkService
{
    private readonly IStorageService storageService;
    private readonly ICsvFileHelper csvHelper;
    private readonly ApplicationDbContext dbContext;
    private readonly ILogger<BulkService> logger;

    public BulkService(
        IStorageService storageService,
        ICsvFileHelper csvHelper,
        ApplicationDbContext dbContext,
        ILogger<BulkService> logger)
    {
        this.storageService = storageService;
        this.csvHelper = csvHelper;
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task<JobEvent> PreprocessingAsync(IFormFile formFile, CancellationToken cancellationToken)
    {
        var uniqueName = Guid.CreateVersion7().ToString();

        var jobEvent = new JobEvent
        {
            JobEventId = Guid.NewGuid(),
            JobEventStatus = BulkProcessStatus.Idle,
            InputFilePath = $"Input_{uniqueName}.csv",
            OutputFilePath = $"Output_{uniqueName}.csv"
        };

        await this.dbContext.AddAsync(jobEvent, cancellationToken);
        await this.dbContext.SaveChangesAsync(cancellationToken);

        using var fileStream = formFile.OpenReadStream();
        await this.storageService.SaveStreamToLocalFile(fileStream, jobEvent.InputFilePath);

        return jobEvent;
    }

    public async Task PostProcessingAsync(JobEvent jobEvent, string hangfireJobId, CancellationToken cancellationToken)
    {
        this.dbContext.Attach(jobEvent);
        this.dbContext.Entry(jobEvent).Property(j => j.HangfireJobId).IsModified = true;

        jobEvent.HangfireJobId = hangfireJobId;

        await this.dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<FileStream> DownloadOutputCsvFileAsync(JobEvent jobEvent, CancellationToken cancellationToken)
    {
        if (!this.storageService.IsFileExistsLocally(jobEvent.OutputFilePath))
        {
            throw new InvalidOperationException("Output file path doesn't exists.");
        }

        return this.storageService.OpenLocalFile(jobEvent.OutputFilePath);
    }

    public async Task ProcessBulkImportAsync<TRequest, TDto>(
        JobEvent jobEvent, 
        Func<TRequest, CancellationToken, Task<TDto>> processRowAsync,
        CancellationToken cancellationToken)
    {
        this.logger.LogInformation($"{nameof(ProcessBulkImportAsync)} has started.");

        using var fileStream = this.storageService.OpenLocalFile(jobEvent.InputFilePath);

        var errors = new List<BulkImportError<TRequest>>();

        await foreach (var request in this.csvHelper.ReadRecordsAsync<TRequest>(fileStream, cancellationToken))
        {
            try
            {
                await processRowAsync(request, cancellationToken);
            }
            catch (Exception exception)
            {
                errors.Add(new (request, exception.Message));
                this.logger.LogError(exception, "Something went wrong while importing users.");
            }
        }

        this.logger.LogInformation("Row process is completed.");

        this.dbContext.Attach(jobEvent);
        this.dbContext.Entry(jobEvent).Property(j => j.JobEventStatus).IsModified = true;

        if (errors.Count == 0)
        {
            jobEvent.JobEventStatus = BulkProcessStatus.Completed;
        }
        else
        {
            jobEvent.JobEventStatus = BulkProcessStatus.Failed;

            using var outputFileStream = await this.csvHelper.WriteRecordsAsync(errors, cancellationToken);
            await this.storageService.SaveStreamToLocalFile(outputFileStream, jobEvent.OutputFilePath);

            this.logger.LogInformation("Output file has been saved.");
        }

        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("JobEvent has been completed.");
    }
}
