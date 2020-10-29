using System;
using CSharpFunctionalExtensions;
using MediatR;

namespace VotRomania.Commands
{
    public class RestartImportJob : IRequest<Result>
    {
        public Guid JobId { get; }

        public RestartImportJob(Guid jobId)
        {
            JobId = jobId;

        }
    }
}