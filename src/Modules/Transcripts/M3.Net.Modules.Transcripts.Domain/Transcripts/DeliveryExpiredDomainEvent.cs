using M3.Net.Common.Domain;

namespace M3.Net.Modules.Transcripts.Domain.Transcripts;

public sealed class DeliveryExpiredDomainEvent(
    Guid transcriptDeliveryId,
    Guid transcriptId,
    Guid userId,
    DateTime expiredAt) : DomainEvent
{
    public Guid TranscriptDeliveryId { get; init; } = transcriptDeliveryId;
    public Guid TranscriptId { get; init; } = transcriptId;
    public Guid UserId { get; init; } = userId;
    public DateTime ExpiredAt { get; init; } = expiredAt;
}
