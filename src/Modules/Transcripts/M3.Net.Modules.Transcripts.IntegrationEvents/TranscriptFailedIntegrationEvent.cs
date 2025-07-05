using M3.Net.Common.Application.EventBus;

namespace M3.Net.Modules.Transcripts.IntegrationEvents;

public sealed class TranscriptFailedIntegrationEvent : IntegrationEvent
{
    public TranscriptFailedIntegrationEvent(
        Guid id,
        DateTime occurredOnUtc,
        Guid requestId,
        Guid userId,
        string youTubeUrl,
        string title,
        string errorReason,
        string? errorDetails = null)
        : base(id, occurredOnUtc)
    {
        RequestId = requestId;
        UserId = userId;
        YouTubeUrl = youTubeUrl;
        Title = title;
        ErrorReason = errorReason;
        ErrorDetails = errorDetails;
    }

    public Guid RequestId { get; init; }

    public Guid UserId { get; init; }

    public string YouTubeUrl { get; init; }

    public string Title { get; init; }

    public string ErrorReason { get; init; }

    public string? ErrorDetails { get; init; }
}
