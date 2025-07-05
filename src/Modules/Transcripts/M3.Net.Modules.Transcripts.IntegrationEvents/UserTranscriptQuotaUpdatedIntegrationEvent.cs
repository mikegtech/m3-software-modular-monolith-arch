using M3.Net.Common.Application.EventBus;

namespace M3.Net.Modules.Transcripts.IntegrationEvents;

public sealed class UserTranscriptQuotaUpdatedIntegrationEvent : IntegrationEvent
{
    public UserTranscriptQuotaUpdatedIntegrationEvent(
        Guid id,
        DateTime occurredOnUtc,
        Guid userId,
        int quotaUsed,
        int quotaLimit,
        DateTime periodStart,
        DateTime periodEnd)
        : base(id, occurredOnUtc)
    {
        UserId = userId;
        QuotaUsed = quotaUsed;
        QuotaLimit = quotaLimit;
        PeriodStart = periodStart;
        PeriodEnd = periodEnd;
    }

    public Guid UserId { get; init; }

    public int QuotaUsed { get; init; }

    public int QuotaLimit { get; init; }

    public DateTime PeriodStart { get; init; }

    public DateTime PeriodEnd { get; init; }
}
