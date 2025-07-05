using M3.Net.Common.Domain;

namespace M3.Net.Modules.Transcripts.Domain.Events;

public sealed class TranscriptDeliveryReadyDomainEvent : DomainEvent
{
    public TranscriptDeliveryReadyDomainEvent(
        Guid deliveryId,
        Guid transcriptId,
        Guid userId,
        string accessToken)
    {
        DeliveryId = deliveryId;
        TranscriptId = transcriptId;
        UserId = userId;
        AccessToken = accessToken;
    }

    public Guid DeliveryId { get; init; }

    public Guid TranscriptId { get; init; }

    public Guid UserId { get; init; }

    public string AccessToken { get; init; }
}

public sealed class UserNotificationSentDomainEvent : DomainEvent
{
    public UserNotificationSentDomainEvent(
        Guid deliveryId,
        Guid transcriptId,
        Guid userId,
        string deliveryMethod,
        DateTime deliveredAt)
    {
        DeliveryId = deliveryId;
        TranscriptId = transcriptId;
        UserId = userId;
        DeliveryMethod = deliveryMethod;
        DeliveredAt = deliveredAt;
    }

    public Guid DeliveryId { get; init; }

    public Guid TranscriptId { get; init; }

    public Guid UserId { get; init; }

    public string DeliveryMethod { get; init; }

    public DateTime DeliveredAt { get; init; }
}

public sealed class TranscriptAccessedDomainEvent : DomainEvent
{
    public TranscriptAccessedDomainEvent(
        Guid deliveryId,
        Guid transcriptId,
        Guid userId,
        int accessCount,
        DateTime accessedAt)
    {
        DeliveryId = deliveryId;
        TranscriptId = transcriptId;
        UserId = userId;
        AccessCount = accessCount;
        AccessedAt = accessedAt;
    }

    public Guid DeliveryId { get; init; }

    public Guid TranscriptId { get; init; }

    public Guid UserId { get; init; }

    public int AccessCount { get; init; }

    public DateTime AccessedAt { get; init; }
}

public sealed class DeliveryExpiredDomainEvent : DomainEvent
{
    public DeliveryExpiredDomainEvent(
        Guid deliveryId,
        Guid transcriptId,
        Guid userId,
        DateTime expiredAt)
    {
        DeliveryId = deliveryId;
        TranscriptId = transcriptId;
        UserId = userId;
        ExpiredAt = expiredAt;
    }

    public Guid DeliveryId { get; init; }

    public Guid TranscriptId { get; init; }

    public Guid UserId { get; init; }

    public DateTime ExpiredAt { get; init; }
}
