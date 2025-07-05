using M3.Net.Common.Domain;

namespace M3.Net.Modules.Transcripts.Domain.Transcripts;

public static class TranscriptErrors
{
    public static class TranscriptRequest
    {
        public static readonly Error InvalidYouTubeUrl = Error.Failure(
            "TranscriptRequest.InvalidYouTubeUrl",
            "The provided YouTube URL is invalid or cannot be accessed");

        public static readonly Error VideoTooLong = Error.Failure(
            "TranscriptRequest.VideoTooLong",
            "The video duration exceeds the maximum allowed length of 4 hours");

        public static readonly Error DuplicateRequest = Error.Conflict(
            "TranscriptRequest.DuplicateRequest",
            "A transcript request for this video already exists within the last 24 hours");

        public static readonly Error QuotaExceeded = Error.Failure(
            "TranscriptRequest.QuotaExceeded",
            "Monthly transcript quota has been exceeded for this user");

        public static readonly Error NotFound = Error.NotFound(
            "TranscriptRequest.NotFound",
            "The transcript request was not found");

        public static readonly Error InvalidStatus = Error.Failure(
            "TranscriptRequest.InvalidStatus",
            "The transcript request is in an invalid status for this operation");
    }

    public static class ProcessingJob
    {
        public static readonly Error NotFound = Error.NotFound(
            "ProcessingJob.NotFound",
            "The processing job was not found");

        public static readonly Error InvalidStatus = Error.Failure(
            "ProcessingJob.InvalidStatus",
            "The processing job is in an invalid status for this operation");

        public static readonly Error ProcessingTimeout = Error.Failure(
            "ProcessingJob.ProcessingTimeout",
            "The processing job has timed out after exceeding the maximum allowed time");

        public static readonly Error MaxRetriesExceeded = Error.Failure(
            "ProcessingJob.MaxRetriesExceeded",
            "The processing job has exceeded the maximum number of retry attempts");

        public static readonly Error InvalidProgress = Error.Failure(
            "ProcessingJob.InvalidProgress",
            "Progress percentage must be between 0 and 100");
    }

    public static class Transcript
    {
        public static readonly Error NotFound = Error.NotFound(
            "Transcript.NotFound",
            "The transcript was not found");

        public static readonly Error InvalidContent = Error.Failure(
            "Transcript.InvalidContent",
            "The transcript content is invalid or empty");

        public static readonly Error InvalidConfidenceScore = Error.Failure(
            "Transcript.InvalidConfidenceScore",
            "Confidence score must be between 0 and 1");

        public static readonly Error AlreadyCompleted = Error.Conflict(
            "Transcript.AlreadyCompleted",
            "The transcript has already been completed");

        public static readonly Error NotCompleted = Error.Failure(
            "Transcript.NotCompleted",
            "The transcript has not been completed yet");

        public static readonly Error AccessDenied = Error.Failure(
            "Transcript.AccessDenied",
            "You do not have permission to access this transcript");
    }

    public static class TranscriptDelivery
    {
        public static readonly Error NotFound = Error.NotFound(
            "TranscriptDelivery.NotFound",
            "The transcript delivery was not found");

        public static readonly Error Expired = Error.Failure(
            "TranscriptDelivery.Expired",
            "The transcript delivery has expired");

        public static readonly Error InvalidAccessToken = Error.Failure(
            "TranscriptDelivery.InvalidAccessToken",
            "The provided access token is invalid");

        public static readonly Error AlreadyDelivered = Error.Conflict(
            "TranscriptDelivery.AlreadyDelivered",
            "The transcript has already been delivered");

        public static readonly Error NotReady = Error.Failure(
            "TranscriptDelivery.NotReady",
            "The transcript delivery is not ready yet");
    }
}
