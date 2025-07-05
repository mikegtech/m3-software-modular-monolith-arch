using M3.Net.Common.Domain;

namespace M3.Net.Modules.Transcripts.Domain.Events;

public sealed class TranscriptRequestSubmittedDomainEvent : DomainEvent
{
    public TranscriptRequestSubmittedDomainEvent(
        Guid requestId,
        Guid userId,
        string youTubeUrl,
        DateTime requestedAt)
    {
        RequestId = requestId;
        UserId = userId;
        YouTubeUrl = youTubeUrl;
        RequestedAt = requestedAt;
    }

    public Guid RequestId { get; init; }

    public Guid UserId { get; init; }

    public string YouTubeUrl { get; init; }

    public DateTime RequestedAt { get; init; }
}

public sealed class VideoUrlValidatedDomainEvent : DomainEvent
{
    public VideoUrlValidatedDomainEvent(
        Guid requestId,
        Guid userId,
        string youTubeUrl,
        string title,
        int durationSeconds)
    {
        RequestId = requestId;
        UserId = userId;
        YouTubeUrl = youTubeUrl;
        Title = title;
        DurationSeconds = durationSeconds;
    }

    public Guid RequestId { get; init; }

    public Guid UserId { get; init; }

    public string YouTubeUrl { get; init; }

    public string Title { get; init; }

    public int DurationSeconds { get; init; }
}

public sealed class VideoUrlRejectedDomainEvent : DomainEvent
{
    public VideoUrlRejectedDomainEvent(
        Guid requestId,
        Guid userId,
        string youTubeUrl,
        string reason)
    {
        RequestId = requestId;
        UserId = userId;
        YouTubeUrl = youTubeUrl;
        Reason = reason;
    }

    public Guid RequestId { get; init; }

    public Guid UserId { get; init; }

    public string YouTubeUrl { get; init; }

    public string Reason { get; init; }
}

public sealed class TranscriptProcessingQueuedDomainEvent : DomainEvent
{
    public TranscriptProcessingQueuedDomainEvent(
        Guid requestId,
        Guid userId,
        string youTubeUrl)
    {
        RequestId = requestId;
        UserId = userId;
        YouTubeUrl = youTubeUrl;
    }

    public Guid RequestId { get; init; }

    public Guid UserId { get; init; }

    public string YouTubeUrl { get; init; }
}
