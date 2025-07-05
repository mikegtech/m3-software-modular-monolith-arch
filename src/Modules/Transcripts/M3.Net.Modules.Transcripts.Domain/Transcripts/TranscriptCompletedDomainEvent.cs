using M3.Net.Common.Domain;

namespace M3.Net.Modules.Transcripts.Domain.Transcripts;

public sealed class TranscriptCompletedDomainEvent(
    Guid transcriptId,
    Guid requestId,
    Guid userId,
    DateTime completedAt) : DomainEvent
{
    public Guid TranscriptId { get; init; } = transcriptId;
    public Guid RequestId { get; init; } = requestId;
    public Guid UserId { get; init; } = userId;
    public DateTime CompletedAt { get; init; } = completedAt;
}
