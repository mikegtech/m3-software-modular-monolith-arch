using M3.Net.Common.Domain;

namespace M3.Net.Modules.Transcripts.Domain.Transcripts;

public sealed class TranscriptDeletedDomainEvent(
    Guid transcriptId,
    Guid requestId,
    Guid userId) : DomainEvent
{
    public Guid TranscriptId { get; init; } = transcriptId;
    public Guid RequestId { get; init; } = requestId;
    public Guid UserId { get; init; } = userId;
}
