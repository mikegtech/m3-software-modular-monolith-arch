using M3.Net.Common.Domain;

namespace M3.Net.Modules.Transcripts.Domain.Transcripts;

public sealed class VideoUrlValidatedDomainEvent(
    Guid transcriptRequestId,
    Guid userId,
    string videoUrl,
    string videoTitle,
    double durationSeconds) : DomainEvent
{
    public Guid TranscriptRequestId { get; init; } = transcriptRequestId;
    public Guid UserId { get; init; } = userId;
    public string VideoUrl { get; init; } = videoUrl;
    public string VideoTitle { get; init; } = videoTitle;
    public double DurationSeconds { get; init; } = durationSeconds;
}
