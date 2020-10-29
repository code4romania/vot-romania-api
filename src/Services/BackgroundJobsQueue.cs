using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace VotRomania.Services
{
    public class BackgroundJobsQueue : IBackgroundJobsQueue
    {
        private readonly ConcurrentQueue<Guid> _workItems = new ConcurrentQueue<Guid>();
        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);

        public Result QueueBackgroundWorkItem(Guid jobId)
        {
            return Result.Try(() =>
            {
                _workItems.Enqueue(jobId);
                _signal.Release();
            });
        }

        public async Task<Guid> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _workItems.TryDequeue(out var workItem);

            return workItem;
        }
    }
}
