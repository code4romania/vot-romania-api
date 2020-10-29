using System;
using CSharpFunctionalExtensions;
using MediatR;

namespace VotRomania.Commands
{
    public class CompleteImportJob : IRequest<Result>
    {
        public CompleteImportJob(Guid jobId)
        {
            JobId = jobId;
        }

        public Guid JobId { get; }
    }
}