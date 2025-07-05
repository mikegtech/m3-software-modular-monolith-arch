using M3.Net.Common.Domain;

namespace M3.Net.Modules.Transcripts.Domain.Transcripts;

public sealed class ProcessingJobFailedDomainEvent(
    Guid processingJobId,
    Guid transcriptRequestId,
    string errorMessage,
    DateTime failedAt) : DomainEvent
{
    public Guid ProcessingJobId { get; init; } = processingJobId;
    public Guid TranscriptRequestId { get; init; } = transcriptRequestId;
    public string ErrorMessage { get; init; } = errorMessage;
    public DateTime FailedAt { get; init; } = failedAt;
}
