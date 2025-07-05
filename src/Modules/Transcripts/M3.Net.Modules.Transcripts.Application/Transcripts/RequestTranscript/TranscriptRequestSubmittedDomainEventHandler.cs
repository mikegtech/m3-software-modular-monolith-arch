using M3.Net.Common.Application.EventBus;
using M3.Net.Common.Application.Messaging;
using M3.Net.Modules.Transcripts.Domain.Transcripts;
using M3.Net.Modules.Transcripts.IntegrationEvents;

namespace M3.Net.Modules.Transcripts.Application.Transcripts.RequestTranscript;

internal sealed class TranscriptRequestSubmittedDomainEventHandler
    : DomainEventHandler<TranscriptRequestSubmittedDomainEvent>
{
    private readonly IEventBus _eventBus;

    public TranscriptRequestSubmittedDomainEventHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public override async Task Handle(
        TranscriptRequestSubmittedDomainEvent notification,
        CancellationToken cancellationToken = default)
    {
        // Publish integration event to notify other modules and Python microservice
        await _eventBus.PublishAsync(
            new TranscriptRequestedIntegrationEvent(
                Guid.NewGuid(),
                DateTime.UtcNow,
                notification.TranscriptRequestId,
                notification.UserId,
                notification.VideoUrl,
                string.Empty, // Title will be filled after validation
                0), // Duration will be filled after validation
            cancellationToken);
    }
}
