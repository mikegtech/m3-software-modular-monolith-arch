using M3.Net.Common.Domain;

namespace M3.Net.Modules.Transcripts.Domain.Transcripts;

public sealed class VideoUrlRejectedDomainEvent(
    Guid transcriptRequestId,
    Guid userId,
    string videoUrl,
    string reason) : DomainEvent
{
    public Guid TranscriptRequestId { get; init; } = transcriptRequestId;
    public Guid UserId { get; init; } = userId;
    public string VideoUrl { get; init; } = videoUrl;
    public string Reason { get; init; } = reason;
}
