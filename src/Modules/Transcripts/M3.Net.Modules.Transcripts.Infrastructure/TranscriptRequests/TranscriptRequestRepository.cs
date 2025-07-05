using M3.Net.Modules.Transcripts.Domain.Transcripts;
using M3.Net.Modules.Transcripts.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace M3.Net.Modules.Transcripts.Infrastructure.TranscriptRequests;

internal sealed class TranscriptRequestRepository(TranscriptsDbContext context) : ITranscriptRequestRepository
{
    public async Task<TranscriptRequest?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.TranscriptRequests
            .FirstOrDefaultAsync(tr => tr.Id == id, cancellationToken);
    }

    public async Task<TranscriptRequest?> GetByUserAndUrlAsync(Guid userId, string youTubeUrl, CancellationToken cancellationToken = default)
    {
        return await context.TranscriptRequests
            .FirstOrDefaultAsync(tr => tr.UserId == userId && tr.YouTubeUrl == youTubeUrl, cancellationToken);
    }

    public async Task<IEnumerable<TranscriptRequest>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await context.TranscriptRequests
            .Where(tr => tr.UserId == userId)
            .OrderByDescending(tr => tr.RequestedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TranscriptRequest>> GetPendingValidationAsync(CancellationToken cancellationToken = default)
    {
        return await context.TranscriptRequests
            .Where(tr => tr.Status == TranscriptRequestStatus.Submitted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TranscriptRequest>> GetQueuedForProcessingAsync(CancellationToken cancellationToken = default)
    {
        return await context.TranscriptRequests
            .Where(tr => tr.Status == TranscriptRequestStatus.Validated)
            .ToListAsync(cancellationToken);
    }

    public void Insert(TranscriptRequest transcriptRequest)
    {
        context.TranscriptRequests.Add(transcriptRequest);
    }

    public void Update(TranscriptRequest transcriptRequest)
    {
        context.TranscriptRequests.Update(transcriptRequest);
    }
}
