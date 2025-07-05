using M3.Net.Common.Domain;

namespace M3.Net.Modules.Transcripts.Domain.Transcripts;

public sealed class ProcessingJobTimedOutDomainEvent(
    Guid processingJobId,
    Guid transcriptRequestId,
    DateTime timedOutAt) : DomainEvent
{
    public Guid ProcessingJobId { get; init; } = processingJobId;
    public Guid TranscriptRequestId { get; init; } = transcriptRequestId;
    public DateTime TimedOutAt { get; init; } = timedOutAt;
}
