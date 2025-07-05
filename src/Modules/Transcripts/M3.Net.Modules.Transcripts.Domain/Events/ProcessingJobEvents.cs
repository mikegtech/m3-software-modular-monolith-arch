using M3.Net.Common.Domain;

namespace M3.Net.Modules.Transcripts.Domain.Events;

public sealed class ProcessingJobCreatedDomainEvent : DomainEvent
{
    public ProcessingJobCreatedDomainEvent(
        Guid jobId,
        Guid transcriptRequestId)
    {
        JobId = jobId;
        TranscriptRequestId = transcriptRequestId;
    }

    public Guid JobId { get; init; }

    public Guid TranscriptRequestId { get; init; }
}

public sealed class ProcessingJobStartedDomainEvent : DomainEvent
{
    public ProcessingJobStartedDomainEvent(
        Guid jobId,
        Guid transcriptRequestId,
        DateTime startedAt)
    {
        JobId = jobId;
        TranscriptRequestId = transcriptRequestId;
        StartedAt = startedAt;
    }

    public Guid JobId { get; init; }

    public Guid TranscriptRequestId { get; init; }

    public DateTime StartedAt { get; init; }
}

public sealed class ProcessingProgressUpdatedDomainEvent : DomainEvent
{
    public ProcessingProgressUpdatedDomainEvent(
        Guid jobId,
        Guid transcriptRequestId,
        int progressPercentage,
        DateTime updatedAt)
    {
        JobId = jobId;
        TranscriptRequestId = transcriptRequestId;
        ProgressPercentage = progressPercentage;
        UpdatedAt = updatedAt;
    }

    public Guid JobId { get; init; }

    public Guid TranscriptRequestId { get; init; }

    public int ProgressPercentage { get; init; }

    public DateTime UpdatedAt { get; init; }
}

public sealed class ProcessingJobCompletedDomainEvent : DomainEvent
{
    public ProcessingJobCompletedDomainEvent(
        Guid jobId,
        Guid transcriptRequestId,
        DateTime completedAt)
    {
        JobId = jobId;
        TranscriptRequestId = transcriptRequestId;
        CompletedAt = completedAt;
    }

    public Guid JobId { get; init; }

    public Guid TranscriptRequestId { get; init; }

    public DateTime CompletedAt { get; init; }
}

public sealed class ProcessingJobFailedDomainEvent : DomainEvent
{
    public ProcessingJobFailedDomainEvent(
        Guid jobId,
        Guid transcriptRequestId,
        string errorDetails,
        DateTime failedAt)
    {
        JobId = jobId;
        TranscriptRequestId = transcriptRequestId;
        ErrorDetails = errorDetails;
        FailedAt = failedAt;
    }

    public Guid JobId { get; init; }

    public Guid TranscriptRequestId { get; init; }

    public string ErrorDetails { get; init; }

    public DateTime FailedAt { get; init; }
}

public sealed class ProcessingJobTimedOutDomainEvent : DomainEvent
{
    public ProcessingJobTimedOutDomainEvent(
        Guid jobId,
        Guid transcriptRequestId,
        DateTime timedOutAt)
    {
        JobId = jobId;
        TranscriptRequestId = transcriptRequestId;
        TimedOutAt = timedOutAt;
    }

    public Guid JobId { get; init; }

    public Guid TranscriptRequestId { get; init; }

    public DateTime TimedOutAt { get; init; }
}
