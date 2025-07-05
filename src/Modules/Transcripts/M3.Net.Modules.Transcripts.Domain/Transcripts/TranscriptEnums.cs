namespace M3.Net.Modules.Transcripts.Domain.Transcripts;

public enum TranscriptRequestStatus
{
    Submitted = 1,
    Validating = 2,
    Validated = 3,
    Rejected = 4,
    Queued = 5,
    Processing = 6,
    Completed = 7,
    Failed = 8,
    Cancelled = 9
}

public enum ProcessingJobStatus
{
    Created = 1,
    Queued = 2,
    Started = 3,
    InProgress = 4,
    Completed = 5,
    Failed = 6,
    TimedOut = 7,
    Cancelled = 8
}

public enum TranscriptStatus
{
    Draft = 1,
    Processing = 2,
    Completed = 3,
    Failed = 4,
    Deleted = 5
}

public enum DeliveryStatus
{
    Pending = 1,
    Ready = 2,
    Delivered = 3,
    Accessed = 4,
    Expired = 5
}
