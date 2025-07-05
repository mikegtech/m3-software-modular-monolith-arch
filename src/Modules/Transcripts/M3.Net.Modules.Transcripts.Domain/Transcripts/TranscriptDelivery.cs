using M3.Net.Common.Domain;

namespace M3.Net.Modules.Transcripts.Domain.Transcripts;

public sealed class TranscriptDelivery : Entity
{
    private TranscriptDelivery()
    {
    }

    public Guid Id { get; private set; }

    public Guid TranscriptId { get; private set; }

    public Guid UserId { get; private set; }

    public DeliveryStatus Status { get; private set; }

    public string? DeliveryMethod { get; private set; }

    public string? AccessToken { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? DeliveredAt { get; private set; }

    public DateTime? FirstAccessedAt { get; private set; }

    public DateTime? LastAccessedAt { get; private set; }

    public DateTime? ExpiresAt { get; private set; }

    public int AccessCount { get; private set; }

    public static TranscriptDelivery Create(
        Guid transcriptId,
        Guid userId,
        string deliveryMethod = "web")
    {
        var delivery = new TranscriptDelivery
        {
            Id = Guid.NewGuid(),
            TranscriptId = transcriptId,
            UserId = userId,
            Status = DeliveryStatus.Pending,
            DeliveryMethod = deliveryMethod,
            AccessToken = GenerateAccessToken(),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7), // 7-day expiry
            AccessCount = 0
        };

        return delivery;
    }

    public void MarkAsReady()
    {
        if (Status != DeliveryStatus.Pending)
        {
            throw new InvalidOperationException("Cannot mark delivery as ready when not pending");
        }

        Status = DeliveryStatus.Ready;

        Raise(new TranscriptDeliveryReadyDomainEvent(
            Id,
            TranscriptId,
            UserId,
            AccessToken!));
    }

    public void MarkAsDelivered()
    {
        if (Status != DeliveryStatus.Ready)
        {
            throw new InvalidOperationException("Cannot mark delivery as delivered when not ready");
        }

        Status = DeliveryStatus.Delivered;
        DeliveredAt = DateTime.UtcNow;

        Raise(new UserNotificationSentDomainEvent(
            Id,
            TranscriptId,
            UserId,
            DeliveryMethod!,
            DeliveredAt.Value));
    }

    public void RecordAccess()
    {
        if (Status != DeliveryStatus.Delivered && Status != DeliveryStatus.Accessed)
        {
            throw new InvalidOperationException("Cannot record access for delivery not delivered");
        }

        if (IsExpired())
        {
            throw new InvalidOperationException("Cannot access expired delivery");
        }

        Status = DeliveryStatus.Accessed;
        AccessCount++;
        LastAccessedAt = DateTime.UtcNow;

        if (FirstAccessedAt == null)
        {
            FirstAccessedAt = DateTime.UtcNow;
        }

        Raise(new TranscriptAccessedDomainEvent(
            Id,
            TranscriptId,
            UserId,
            AccessCount,
            LastAccessedAt.Value));
    }

    public void Expire()
    {
        Status = DeliveryStatus.Expired;
        ExpiresAt = DateTime.UtcNow;

        Raise(new DeliveryExpiredDomainEvent(
            Id,
            TranscriptId,
            UserId,
            ExpiresAt.Value));
    }

    public bool IsExpired()
    {
        return ExpiresAt.HasValue && DateTime.UtcNow > ExpiresAt.Value;
    }

    public void ExtendExpiry(int additionalDays)
    {
        if (additionalDays <= 0)
        {
            throw new ArgumentException("Additional days must be positive", nameof(additionalDays));
        }

        ExpiresAt = (ExpiresAt ?? DateTime.UtcNow).AddDays(additionalDays);

        if (Status == DeliveryStatus.Expired)
        {
            Status = DeliveryStatus.Delivered;
        }
    }

    private static string GenerateAccessToken()
    {
        return Guid.NewGuid().ToString("N")[..16]; // 16-character token
    }
}
