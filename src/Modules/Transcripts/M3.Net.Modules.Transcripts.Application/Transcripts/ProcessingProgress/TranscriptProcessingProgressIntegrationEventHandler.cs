using M3.Net.Common.Application.EventBus;
using M3.Net.Common.Application.Messaging;
using M3.Net.Modules.Transcripts.IntegrationEvents;
using Microsoft.Extensions.Logging;

namespace M3.Net.Modules.Transcripts.Application.Transcripts.ProcessingProgress;

internal sealed class TranscriptProcessingProgressIntegrationEventHandler
    : IntegrationEventHandler<TranscriptProcessingProgressIntegrationEvent>
{
    private readonly ILogger<TranscriptProcessingProgressIntegrationEventHandler> _logger;

    public TranscriptProcessingProgressIntegrationEventHandler(
        ILogger<TranscriptProcessingProgressIntegrationEventHandler> logger)
    {
        _logger = logger;
    }

    public override async Task Handle(
        TranscriptProcessingProgressIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Transcript processing progress received for request {RequestId}: {Status} ({Progress}%) - {Message}",
            integrationEvent.RequestId,
            integrationEvent.Status,
            integrationEvent.ProgressPercentage,
            integrationEvent.Message);

        // TODO: Update transcript request status in database
        // TODO: Potentially send real-time updates to clients via SignalR
        // TODO: Store progress history for audit/debugging

        await Task.CompletedTask;
    }
}
