using M3.Net.Modules.Transcripts.Domain.Transcripts;
using M3.Net.Modules.Transcripts.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace M3.Net.Modules.Transcripts.Infrastructure.TranscriptDeliveries;

internal sealed class TranscriptDeliveryRepository : ITranscriptDeliveryRepository
{
    private readonly TranscriptsDbContext _context;

    public TranscriptDeliveryRepository(TranscriptsDbContext context)
    {
        _context = context;
    }

    public async Task<TranscriptDelivery?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.TranscriptDeliveries
            .FirstOrDefaultAsync(td => td.Id == id, cancellationToken);
    }

    public async Task<TranscriptDelivery?> GetByTranscriptIdAsync(Guid transcriptId, CancellationToken cancellationToken = default)
    {
        return await _context.TranscriptDeliveries
            .FirstOrDefaultAsync(td => td.TranscriptId == transcriptId, cancellationToken);
    }

    public async Task<TranscriptDelivery?> GetByAccessTokenAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        return await _context.TranscriptDeliveries
            .FirstOrDefaultAsync(td => td.AccessToken == accessToken, cancellationToken);
    }

    public async Task<IEnumerable<TranscriptDelivery>> GetExpiredDeliveriesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.TranscriptDeliveries
            .Where(td => td.ExpiresAt.HasValue && td.ExpiresAt.Value < DateTime.UtcNow)
            .ToListAsync(cancellationToken);
    }

    public void Insert(TranscriptDelivery transcriptDelivery)
    {
        _context.TranscriptDeliveries.Add(transcriptDelivery);
    }

    public void Update(TranscriptDelivery transcriptDelivery)
    {
        _context.TranscriptDeliveries.Update(transcriptDelivery);
    }
}
