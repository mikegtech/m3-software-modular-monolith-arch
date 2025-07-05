using M3.Net.Common.Domain;

namespace M3.Net.Modules.Transcripts.Domain.Transcripts;

public sealed class TranscriptContentReceivedDomainEvent(
    Guid transcriptId,
    Guid requestId,
    Guid userId,
    int contentLength,
    int wordCount,
    string language,
    decimal confidenceScore) : DomainEvent
{
    public Guid TranscriptId { get; init; } = transcriptId;
    public Guid RequestId { get; init; } = requestId;
    public Guid UserId { get; init; } = userId;
    public int ContentLength { get; init; } = contentLength;
    public int WordCount { get; init; } = wordCount;
    public string Language { get; init; } = language;
    public decimal ConfidenceScore { get; init; } = confidenceScore;
}
