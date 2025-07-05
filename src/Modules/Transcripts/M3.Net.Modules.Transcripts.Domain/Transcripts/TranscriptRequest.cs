using M3.Net.Common.Domain;

namespace M3.Net.Modules.Transcripts.Domain.Transcripts;

public sealed class TranscriptRequest : Entity
{
    private TranscriptRequest()
    {
    }

    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public string YouTubeUrl { get; private set; } = string.Empty;

    public string? Title { get; private set; }

    public string? Description { get; private set; }

    public int? DurationSeconds { get; private set; }

    public TranscriptRequestStatus Status { get; private set; }

    public string? RejectionReason { get; private set; }

    public DateTime RequestedAt { get; private set; }

    public DateTime? ValidatedAt { get; private set; }

    public DateTime? CompletedAt { get; private set; }

    public static TranscriptRequest Create(
        Guid userId,
        string youTubeUrl,
        string? title = null,
        string? description = null)
    {
        var request = new TranscriptRequest
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            YouTubeUrl = youTubeUrl,
            Title = title,
            Description = description,
            Status = TranscriptRequestStatus.Submitted,
            RequestedAt = DateTime.UtcNow
        };

        request.Raise(new TranscriptRequestSubmittedDomainEvent(
            request.Id,
            request.UserId,
            request.YouTubeUrl,
            request.RequestedAt));

        return request;
    }

    public void Validate(string title, int durationSeconds)
    {
        if (Status != TranscriptRequestStatus.Submitted && Status != TranscriptRequestStatus.Validating)
        {
            throw new InvalidOperationException("Cannot validate request in current status");
        }

        Title = title;
        DurationSeconds = durationSeconds;
        Status = TranscriptRequestStatus.Validated;
        ValidatedAt = DateTime.UtcNow;

        Raise(new VideoUrlValidatedDomainEvent(
            Id,
            UserId,
            YouTubeUrl,
            title,
            durationSeconds));
    }

    public void Reject(string reason)
    {
        if (Status != TranscriptRequestStatus.Submitted && Status != TranscriptRequestStatus.Validating)
        {
            throw new InvalidOperationException("Cannot reject request in current status");
        }

        Status = TranscriptRequestStatus.Rejected;
        RejectionReason = reason;
        ValidatedAt = DateTime.UtcNow;

        Raise(new VideoUrlRejectedDomainEvent(
            Id,
            UserId,
            YouTubeUrl,
            reason));
    }

    public void MarkAsQueued()
    {
        if (Status != TranscriptRequestStatus.Validated)
        {
            throw new InvalidOperationException("Cannot queue request that is not validated");
        }

        Status = TranscriptRequestStatus.Queued;

        Raise(new TranscriptProcessingQueuedDomainEvent(
            Id,
            UserId,
            YouTubeUrl));
    }

    public void MarkAsProcessing()
    {
        if (Status != TranscriptRequestStatus.Queued)
        {
            throw new InvalidOperationException("Cannot start processing request that is not queued");
        }

        Status = TranscriptRequestStatus.Processing;
    }

    public void MarkAsCompleted()
    {
        if (Status != TranscriptRequestStatus.Processing)
        {
            throw new InvalidOperationException("Cannot complete request that is not processing");
        }

        Status = TranscriptRequestStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }

    public void MarkAsFailed()
    {
        Status = TranscriptRequestStatus.Failed;
        CompletedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == TranscriptRequestStatus.Completed)
        {
            throw new InvalidOperationException("Cannot cancel completed request");
        }

        Status = TranscriptRequestStatus.Cancelled;
        CompletedAt = DateTime.UtcNow;
    }
}
