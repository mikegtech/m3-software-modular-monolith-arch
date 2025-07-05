using M3.Net.Common.Domain;

namespace M3.Net.Modules.Transcripts.Domain.Transcripts;

public sealed class ProcessingProgressUpdatedDomainEvent(
    Guid processingJobId,
    Guid transcriptRequestId,
    int progressPercentage,
    DateTime lastUpdate) : DomainEvent
{
    public Guid ProcessingJobId { get; init; } = processingJobId;
    public Guid TranscriptRequestId { get; init; } = transcriptRequestId;
    public int ProgressPercentage { get; init; } = progressPercentage;
    public DateTime LastUpdate { get; init; } = lastUpdate;
}
