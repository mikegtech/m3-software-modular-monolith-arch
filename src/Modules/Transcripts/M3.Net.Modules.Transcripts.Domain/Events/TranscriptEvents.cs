using M3.Net.Common.Domain;

namespace M3.Net.Modules.Transcripts.Domain.Events;

public sealed class TranscriptContentReceivedDomainEvent : DomainEvent
{
    public TranscriptContentReceivedDomainEvent(
        Guid transcriptId,
        Guid requestId,
        Guid userId,
        int contentLength,
        int wordCount,
        string language,
        decimal confidenceScore)
    {
        TranscriptId = transcriptId;
        RequestId = requestId;
        UserId = userId;
        ContentLength = contentLength;
        WordCount = wordCount;
        Language = language;
        ConfidenceScore = confidenceScore;
    }

    public Guid TranscriptId { get; init; }

    public Guid RequestId { get; init; }

    public Guid UserId { get; init; }

    public int ContentLength { get; init; }

    public int WordCount { get; init; }

    public string Language { get; init; }

    public decimal ConfidenceScore { get; init; }
}

public sealed class TranscriptCompletedDomainEvent : DomainEvent
{
    public TranscriptCompletedDomainEvent(
        Guid transcriptId,
        Guid requestId,
        Guid userId,
        DateTime completedAt)
    {
        TranscriptId = transcriptId;
        RequestId = requestId;
        UserId = userId;
        CompletedAt = completedAt;
    }

    public Guid TranscriptId { get; init; }

    public Guid RequestId { get; init; }

    public Guid UserId { get; init; }

    public DateTime CompletedAt { get; init; }
}

public sealed class TranscriptFailedDomainEvent : DomainEvent
{
    public TranscriptFailedDomainEvent(
        Guid transcriptId,
        Guid requestId,
        Guid userId,
        DateTime failedAt)
    {
        TranscriptId = transcriptId;
        RequestId = requestId;
        UserId = userId;
        FailedAt = failedAt;
    }

    public Guid TranscriptId { get; init; }

    public Guid RequestId { get; init; }

    public Guid UserId { get; init; }

    public DateTime FailedAt { get; init; }
}

public sealed class TranscriptDeletedDomainEvent : DomainEvent
{
    public TranscriptDeletedDomainEvent(
        Guid transcriptId,
        Guid requestId,
        Guid userId)
    {
        TranscriptId = transcriptId;
        RequestId = requestId;
        UserId = userId;
    }

    public Guid TranscriptId { get; init; }

    public Guid RequestId { get; init; }

    public Guid UserId { get; init; }
}
