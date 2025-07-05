using M3.Net.Common.Application.EventBus;

namespace M3.Net.Modules.Transcripts.IntegrationEvents;

public sealed class TranscriptRequestedIntegrationEvent : IntegrationEvent
{
    public TranscriptRequestedIntegrationEvent(
        Guid id,
        DateTime occurredOnUtc,
        Guid requestId,
        Guid userId,
        string youTubeUrl,
        string title,
        int durationSeconds)
        : base(id, occurredOnUtc)
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
