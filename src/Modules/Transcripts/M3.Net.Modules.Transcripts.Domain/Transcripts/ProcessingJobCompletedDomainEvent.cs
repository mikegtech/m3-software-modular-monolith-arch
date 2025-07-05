using M3.Net.Common.Domain;

namespace M3.Net.Modules.Transcripts.Domain.Transcripts;

public sealed class ProcessingJobCompletedDomainEvent(
    Guid processingJobId,
    Guid transcriptRequestId,
    DateTime completedAt) : DomainEvent
{
    public Guid ProcessingJobId { get; init; } = processingJobId;
    public Guid TranscriptRequestId { get; init; } = transcriptRequestId;
    public DateTime CompletedAt { get; init; } = completedAt;
}
