using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace VotRomania.Services
{
    public interface IBackgroundJobsQueue
    {
        Result QueueBackgroundWorkItem(Guid workItem);

        Task<Guid> DequeueAsync(CancellationToken cancellationToken);
    }
}