using M3.Net.Common.Domain;

namespace M3.Net.Modules.Transcripts.Domain.Transcripts;

public sealed class ProcessingJobStartedDomainEvent(
    Guid processingJobId,
    Guid transcriptRequestId,
    DateTime startedAt) : DomainEvent
{
    public Guid ProcessingJobId { get; init; } = processingJobId;
    public Guid TranscriptRequestId { get; init; } = transcriptRequestId;
    public DateTime StartedAt { get; init; } = startedAt;
}
