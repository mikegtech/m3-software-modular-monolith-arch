using M3.Net.Modules.Transcripts.Domain.Transcripts;
using M3.Net.Modules.Transcripts.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace M3.Net.Modules.Transcripts.Infrastructure.Transcripts;

internal sealed class TranscriptRepository : ITranscriptRepository
{
    private readonly TranscriptsDbContext _context;

    public TranscriptRepository(TranscriptsDbContext context)
    {
        _context = context;
    }

    public async Task<Transcript?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Transcripts
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<Transcript?> GetByRequestIdAsync(Guid requestId, CancellationToken cancellationToken = default)
    {
        return await _context.Transcripts
            .FirstOrDefaultAsync(t => t.RequestId == requestId, cancellationToken);
    }

    public async Task<IEnumerable<Transcript>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Transcripts
            .Where(t => t.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Transcript>> GetCompletedTranscriptsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Transcripts
            .Where(t => t.Status == TranscriptStatus.Completed)
            .ToListAsync(cancellationToken);
    }

    public void Insert(Transcript transcript)
    {
        _context.Transcripts.Add(transcript);
    }

    public void Update(Transcript transcript)
    {
        _context.Transcripts.Update(transcript);
    }
}
