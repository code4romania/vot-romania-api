using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Http;
using VotRomania.Models;
using VotRomania.Stores.Entities;

namespace VotRomania.Stores
{
    public interface IImportJobsRepository
    {
        Task<Result> InsertInJobTable(Guid jobId, IFormFile requestFile);
        Task<Result<bool>> HasImportJobInProgress();
        Task<Result> CancelImportJob(Guid jobId);
        Task<Result<JobStatusModel>> GetImportJobStatus(Guid jobId);
        Task<Result> UpdateJobStatus(Guid jobId, JobStatus jobJobStatus);
        Task<Result<JobStatusModel>> GetCurrentImportJob();
        Task<Result> RestartJob(Guid jobId);
    }
}
