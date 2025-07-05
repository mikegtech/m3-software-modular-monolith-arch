using M3.Net.Common.Domain;

namespace M3.Net.Modules.Transcripts.Domain.Transcripts;

public sealed class UserNotificationSentDomainEvent(
    Guid transcriptDeliveryId,
    Guid transcriptId,
    Guid userId,
    string deliveryMethod,
    DateTime deliveredAt) : DomainEvent
{
    public Guid TranscriptDeliveryId { get; init; } = transcriptDeliveryId;
    public Guid TranscriptId { get; init; } = transcriptId;
    public Guid UserId { get; init; } = userId;
    public string DeliveryMethod { get; init; } = deliveryMethod;
    public DateTime DeliveredAt { get; init; } = deliveredAt;
}
