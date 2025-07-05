using M3.Net.Common.Application.EventBus;

namespace M3.Net.Modules.Transcripts.IntegrationEvents;

public sealed class TranscriptCompletedIntegrationEvent : IntegrationEvent
{
    public TranscriptCompletedIntegrationEvent(
        Guid id,
        DateTime occurredOnUtc,
        Guid transcriptId,
        Guid requestId,
        Guid userId,
        string youTubeUrl,
        string title,
        int wordCount,
        int durationSeconds,
        string language,
        decimal confidenceScore)
        : base(id, occurredOnUtc)
    {
        TranscriptId = transcriptId;
        RequestId = requestId;
        UserId = userId;
        YouTubeUrl = youTubeUrl;
        Title = title;
        WordCount = wordCount;
        DurationSeconds = durationSeconds;
        Language = language;
        ConfidenceScore = confidenceScore;
    }

    public Guid TranscriptId { get; init; }

    public Guid RequestId { get; init; }

    public Guid UserId { get; init; }

    public string YouTubeUrl { get; init; }

    public string Title { get; init; }

    public int WordCount { get; init; }

    public int DurationSeconds { get; init; }

    public string Language { get; init; }

    public decimal ConfidenceScore { get; init; }
}
