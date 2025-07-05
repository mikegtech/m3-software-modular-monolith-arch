using M3.Net.Common.Domain;

namespace M3.Net.Modules.Transcripts.Domain.Transcripts;

public sealed class TranscriptProcessingQueuedDomainEvent(
    Guid transcriptRequestId,
    Guid userId,
    string videoUrl) : DomainEvent
{
    public Guid TranscriptRequestId { get; init; } = transcriptRequestId;
    public Guid UserId { get; init; } = userId;
    public string VideoUrl { get; init; } = videoUrl;
}
