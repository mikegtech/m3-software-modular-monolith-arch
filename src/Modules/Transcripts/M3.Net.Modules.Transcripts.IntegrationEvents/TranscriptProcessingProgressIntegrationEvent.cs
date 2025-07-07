using M3.Net.Common.Application.EventBus;

namespace M3.Net.Modules.Transcripts.IntegrationEvents;

public sealed class TranscriptProcessingProgressIntegrationEvent : IntegrationEvent
{
    public TranscriptProcessingProgressIntegrationEvent(
        Guid id,
        DateTime occurredOnUtc,
        Guid requestId,
        string status,
        int progressPercentage,
        string? message)
        : base(id, occurredOnUtc)
    {
        RequestId = requestId;
        Status = status;
        ProgressPercentage = progressPercentage;
        Message = message;
    }

    public Guid RequestId { get; init; }

    public string Status { get; init; }

    public int ProgressPercentage { get; init; }

    public string? Message { get; init; }
}
