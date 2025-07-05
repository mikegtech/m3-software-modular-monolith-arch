using M3.Net.Common.Domain;

namespace M3.Net.Modules.Transcripts.Domain.Transcripts;

public sealed class ProcessingJob : Entity
{
    private ProcessingJob()
    {
    }

    public Guid Id { get; private set; }

    public Guid TranscriptRequestId { get; private set; }

    public ProcessingJobStatus Status { get; private set; }

    public int ProgressPercentage { get; private set; }

    public string? ErrorDetails { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? StartedAt { get; private set; }

    public DateTime? CompletedAt { get; private set; }

    public DateTime? LastProgressUpdate { get; private set; }

    public int RetryCount { get; private set; }

    public static ProcessingJob Create(Guid transcriptRequestId)
    {
        var job = new ProcessingJob
        {
            Id = Guid.NewGuid(),
            TranscriptRequestId = transcriptRequestId,
            Status = ProcessingJobStatus.Created,
            ProgressPercentage = 0,
            CreatedAt = DateTime.UtcNow,
            RetryCount = 0
        };

        job.Raise(new ProcessingJobCreatedDomainEvent(
            job.Id,
            job.TranscriptRequestId));

        return job;
    }

    public void Start()
    {
        if (Status != ProcessingJobStatus.Created && Status != ProcessingJobStatus.Queued)
        {
            throw new InvalidOperationException("Cannot start job in current status");
        }

        Status = ProcessingJobStatus.Started;
        StartedAt = DateTime.UtcNow;
        LastProgressUpdate = DateTime.UtcNow;

        Raise(new ProcessingJobStartedDomainEvent(
            Id,
            TranscriptRequestId,
            StartedAt.Value));
    }

    public void UpdateProgress(int progressPercentage)
    {
        if (Status != ProcessingJobStatus.Started && Status != ProcessingJobStatus.InProgress)
        {
            throw new InvalidOperationException("Cannot update progress for job not in progress");
        }

        if (progressPercentage < 0 || progressPercentage > 100)
        {
            throw new ArgumentException("Progress percentage must be between 0 and 100");
        }

        Status = ProcessingJobStatus.InProgress;
        ProgressPercentage = progressPercentage;
        LastProgressUpdate = DateTime.UtcNow;

        Raise(new ProcessingProgressUpdatedDomainEvent(
            Id,
            TranscriptRequestId,
            progressPercentage,
            LastProgressUpdate.Value));
    }

    public void Complete()
    {
        if (Status != ProcessingJobStatus.Started && Status != ProcessingJobStatus.InProgress)
        {
            throw new InvalidOperationException("Cannot complete job not in progress");
        }

        Status = ProcessingJobStatus.Completed;
        ProgressPercentage = 100;
        CompletedAt = DateTime.UtcNow;

        Raise(new ProcessingJobCompletedDomainEvent(
            Id,
            TranscriptRequestId,
            CompletedAt.Value));
    }

    public void Fail(string errorDetails)
    {
        Status = ProcessingJobStatus.Failed;
        ErrorDetails = errorDetails;
        CompletedAt = DateTime.UtcNow;

        Raise(new ProcessingJobFailedDomainEvent(
            Id,
            TranscriptRequestId,
            errorDetails,
            CompletedAt.Value));
    }

    public void Timeout()
    {
        Status = ProcessingJobStatus.TimedOut;
        ErrorDetails = "Processing timed out after maximum allowed time";
        CompletedAt = DateTime.UtcNow;

        Raise(new ProcessingJobTimedOutDomainEvent(
            Id,
            TranscriptRequestId,
            CompletedAt.Value));
    }

    public void Cancel()
    {
        if (Status == ProcessingJobStatus.Completed)
        {
            throw new InvalidOperationException("Cannot cancel completed job");
        }

        Status = ProcessingJobStatus.Cancelled;
        CompletedAt = DateTime.UtcNow;
    }

    public bool CanRetry()
    {
        return Status == ProcessingJobStatus.Failed && RetryCount < 3;
    }

    public void IncrementRetryCount()
    {
        RetryCount++;
    }
}
