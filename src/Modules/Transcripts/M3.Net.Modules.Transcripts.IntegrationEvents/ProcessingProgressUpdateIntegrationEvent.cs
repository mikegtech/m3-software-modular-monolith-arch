using M3.Net.Common.Application.EventBus;

namespace M3.Net.Modules.Transcripts.IntegrationEvents;

public sealed class ProcessingProgressUpdateIntegrationEvent : IntegrationEvent
{
    public ProcessingProgressUpdateIntegrationEvent(
        Guid id,
        DateTime occurredOnUtc,
        Guid requestId,
        Guid jobId,
        int progressPercentage,
        string? statusMessage = null,
        TimeSpan? estimatedTimeRemaining = null)
        : base(id, occurredOnUtc)
    {
        RequestId = requestId;
        JobId = jobId;
        ProgressPercentage = progressPercentage;
        StatusMessage = statusMessage;
        EstimatedTimeRemaining = estimatedTimeRemaining;
    }

    public Guid RequestId { get; init; }

    public Guid JobId { get; init; }

    public int ProgressPercentage { get; init; }

    public string? StatusMessage { get; init; }

    public TimeSpan? EstimatedTimeRemaining { get; init; }
}
