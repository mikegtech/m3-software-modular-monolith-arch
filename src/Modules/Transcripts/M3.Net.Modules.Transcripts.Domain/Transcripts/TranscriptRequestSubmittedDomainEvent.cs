using M3.Net.Common.Domain;

namespace M3.Net.Modules.Transcripts.Domain.Transcripts;

public sealed class TranscriptRequestSubmittedDomainEvent(
    Guid transcriptRequestId,
    Guid userId,
    string videoUrl,
    DateTime requestedAt) : DomainEvent
{
    public Guid TranscriptRequestId { get; init; } = transcriptRequestId;
    public Guid UserId { get; init; } = userId;
    public string VideoUrl { get; init; } = videoUrl;
    public DateTime RequestedAt { get; init; } = requestedAt;
}
