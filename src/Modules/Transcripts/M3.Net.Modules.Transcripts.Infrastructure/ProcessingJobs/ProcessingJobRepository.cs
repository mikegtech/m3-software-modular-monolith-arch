using M3.Net.Modules.Transcripts.Domain.Transcripts;
using M3.Net.Modules.Transcripts.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace M3.Net.Modules.Transcripts.Infrastructure.ProcessingJobs;

internal sealed class ProcessingJobRepository(TranscriptsDbContext context) : IProcessingJobRepository
{
    public async Task<ProcessingJob?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.ProcessingJobs
            .SingleOrDefaultAsync(pj => pj.Id == id, cancellationToken);
    }

    public async Task<ProcessingJob?> GetByTranscriptRequestAsync(Guid transcriptRequestId, CancellationToken cancellationToken = default)
    {
        return await context.ProcessingJobs
            .SingleOrDefaultAsync(pj => pj.TranscriptRequestId == transcriptRequestId, cancellationToken);
    }

    public async Task<IEnumerable<ProcessingJob>> GetActiveJobsAsync(CancellationToken cancellationToken = default)
    {
        return await context.ProcessingJobs
            .Where(pj => pj.Status == ProcessingJobStatus.InProgress)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProcessingJob>> GetTimedOutJobsAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        DateTime timeoutThreshold = DateTime.UtcNow.Subtract(timeout);
        return await context.ProcessingJobs
            .Where(pj => pj.Status == ProcessingJobStatus.InProgress &&
                        pj.StartedAt.HasValue &&
                        pj.StartedAt.Value < timeoutThreshold)
            .ToListAsync(cancellationToken);
    }

    public void Insert(ProcessingJob processingJob)
    {
        context.ProcessingJobs.Add(processingJob);
    }

    public void Update(ProcessingJob processingJob)
    {
        context.ProcessingJobs.Update(processingJob);
    }
}
