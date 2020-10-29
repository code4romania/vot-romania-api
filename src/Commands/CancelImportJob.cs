using System;
using CSharpFunctionalExtensions;
using MediatR;

namespace VotRomania.Commands
{
    public class CancelImportJob : IRequest<Result>
    {
        public CancelImportJob(Guid jobId)
        {
            JobId = jobId;
        }

        public Guid JobId { get; }
    }
}
