using M3.Net.Common.Domain;

namespace M3.Net.Modules.Transcripts.Domain.Transcripts;

public sealed class TranscriptAccessedDomainEvent(
    Guid transcriptDeliveryId,
    Guid transcriptId,
    Guid userId,
    int accessCount,
    DateTime accessedAt) : DomainEvent
{
    public Guid TranscriptDeliveryId { get; init; } = transcriptDeliveryId;
    public Guid TranscriptId { get; init; } = transcriptId;
    public Guid UserId { get; init; } = userId;
    public int AccessCount { get; init; } = accessCount;
    public DateTime AccessedAt { get; init; } = accessedAt;
}
