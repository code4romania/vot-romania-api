using System;
using CSharpFunctionalExtensions;
using MediatR;
using VotRomania.Models;

namespace VotRomania.Commands
{
    public class GetImportJobStatus : IRequest<Result<JobStatusModel>>
    {
        public GetImportJobStatus(Guid jobId)
        {
            JobId = jobId;
        }

        public Guid JobId { get; }
    }
}
