using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VotRomania.Models;
using VotRomania.Stores.Entities;

namespace VotRomania.Stores
{
    public class ImportJobsRepository : IImportJobsRepository
    {
        private readonly VotRomaniaContext _context;
        private ILogger<ImportJobsRepository> _logger;

        public ImportJobsRepository(VotRomaniaContext context, ILogger<ImportJobsRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result> InsertInJobTable(Guid jobId, IFormFile requestFile)
        {
            var result = await Result.Try(async () =>
            {
                using (var ms = new MemoryStream())
                {
                    requestFile.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    string encodedFile = Convert.ToBase64String(fileBytes);

                    await _context.ImportJobs.AddAsync(new UploadJobsEntity
                    {
                        FileName = requestFile.FileName,
                        Base64File = encodedFile,
                        JobId = jobId.ToString(),
                        JobStatus = JobStatus.NotStarted,
                        // TODO: change here when db supports it
                        // Started = DateTime.Now 
                    });

                    await _context.SaveChangesAsync();
                }
            }, exception => LogException(exception, $"Error in method {nameof(InsertInJobTable)}"));

            return result;
        }

        public async Task<Result<bool>> HasImportJobInProgress()
        {
            var result = await Result.Try(async () =>
            {
                var numberOfJobs = await _context.ImportJobs.CountAsync(x => x.JobStatus == JobStatus.Started || x.JobStatus == JobStatus.NotStarted);
                return numberOfJobs > 0;
            }, e => LogException(e));

            return result;
        }

        public async Task<Result> CancelImportJob(Guid jobId)
        {
            var result = await Result.Try(async () =>
            {
                var job = await _context.ImportJobs.FirstOrDefaultAsync(x => x.JobId == jobId.ToString());

                if (job == null)
                {
                    throw new Exception($"Could not find job with requested id = {jobId}");
                }

                job.JobStatus = JobStatus.Canceled;
                await _context.SaveChangesAsync();
                return Result.Success();
            }, e => LogException(e));

            return result;
        }

        public async Task<Result<JobStatusModel>> GetImportJobStatus(Guid jobId)
        {
            var result = await Result.Try(async () =>
            {
                var job = await _context.ImportJobs
                    .FirstOrDefaultAsync(x => x.JobId == jobId.ToString());

                if (job == null)
                {
                    throw new Exception($"Could not find job with requested id = {jobId}");
                }

                return new JobStatusModel
                {
                    JobId = job.JobId,
                    Status = job.JobStatus,
                    Started = job.Started,
                    Ended = job.Ended,
                };
            }, e => LogException(e));

            return result;
        }

        public async Task<Result> UpdateJobStatus(Guid jobId, JobStatus jobJobStatus)
        {
            var result = await Result.Try(async () =>
            {
                var job = await _context.ImportJobs
                    .FirstOrDefaultAsync(x => x.JobId == jobId.ToString());

                if (job == null)
                {
                    throw new Exception($"Could not find job with requested id = {jobId}");
                }

                job.JobStatus = jobJobStatus;
                await _context.SaveChangesAsync();

            }, e => LogException(e));

            return result;
        }

        public async Task<Result<JobStatusModel>> GetCurrentImportJob()
        {
            var result = await Result.Try(async () =>
            {
                var job = await _context.ImportJobs
                    .Where(x => x.JobStatus == JobStatus.NotStarted || x.JobStatus == JobStatus.Started ||
                                x.JobStatus == JobStatus.Finished)
                    .FirstOrDefaultAsync();

                if (job == null)
                {
                    return null;
                }

                return new JobStatusModel
                {
                    JobId = job.JobId,
                    Status = job.JobStatus,
                    Started = job.Started,
                    Ended = job.Ended,
                    FileName = job.FileName
                };

            }, e => LogException(e));

            return result;
        }

        public async Task<Result> RestartJob(Guid jobId)
        {
            return await Result.Try(async () =>
             {
                 var job = await _context.ImportJobs
                     .Where(x => x.JobId == jobId.ToString())
                     .FirstAsync();

                 job.JobStatus = JobStatus.Started;
                 await _context.SaveChangesAsync();
             });
        }

        private string LogException(Exception exception, string? message = null)
        {
            var exceptionMessage = string.IsNullOrWhiteSpace(message) ? exception.Message : message;
            _logger.LogError(exception, exceptionMessage);
            return exceptionMessage;
        }
    }
}