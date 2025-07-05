using M3.Net.Common.Application.EventBus;

namespace M3.Net.Modules.Transcripts.IntegrationEvents;

public sealed class ProcessingResultReceivedIntegrationEvent : IntegrationEvent
{
    public ProcessingResultReceivedIntegrationEvent(
        Guid id,
        DateTime occurredOnUtc,
        Guid requestId,
        Guid jobId,
        bool success,
        string? transcriptContent = null,
        string? language = null,
        decimal? confidenceScore = null,
        string? errorMessage = null,
        string? errorDetails = null)
        : base(id, occurredOnUtc)
    {
        RequestId = requestId;
        JobId = jobId;
        Success = success;
        TranscriptContent = transcriptContent;
        Language = language;
        ConfidenceScore = confidenceScore;
        ErrorMessage = errorMessage;
        ErrorDetails = errorDetails;
    }

    public Guid RequestId { get; init; }

    public Guid JobId { get; init; }

    public bool Success { get; init; }

    public string? TranscriptContent { get; init; }

    public string? Language { get; init; }

    public decimal? ConfidenceScore { get; init; }

    public string? ErrorMessage { get; init; }

    public string? ErrorDetails { get; init; }
}
