using M3.Net.Common.Application.EventBus;

namespace M3.Net.Modules.Transcripts.IntegrationEvents;

public sealed class TranscriptProcessingCompletedIntegrationEvent : IntegrationEvent
{
    public TranscriptProcessingCompletedIntegrationEvent(
        Guid id,
        DateTime occurredOnUtc,
        Guid requestId,
        bool success,
        string? transcriptContent,
        string? errorMessage)
        : base(id, occurredOnUtc)
    {
        RequestId = requestId;
        Success = success;
        TranscriptContent = transcriptContent;
        ErrorMessage = errorMessage;
    }

    public Guid RequestId { get; init; }

    public bool Success { get; init; }

    public string? TranscriptContent { get; init; }

    public string? ErrorMessage { get; init; }
}
