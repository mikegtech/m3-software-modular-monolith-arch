using M3.Net.Common.Domain;

namespace M3.Net.Modules.Transcripts.Domain.Transcripts;

public sealed class TranscriptDeliveryReadyDomainEvent(
    Guid transcriptDeliveryId,
    Guid transcriptId,
    Guid userId,
    string accessToken) : DomainEvent
{
    public Guid TranscriptDeliveryId { get; init; } = transcriptDeliveryId;
    public Guid TranscriptId { get; init; } = transcriptId;
    public Guid UserId { get; init; } = userId;
    public string AccessToken { get; init; } = accessToken;
}
