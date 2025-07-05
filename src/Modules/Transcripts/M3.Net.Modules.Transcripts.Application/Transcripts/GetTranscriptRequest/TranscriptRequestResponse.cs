using M3.Net.Modules.Transcripts.Domain.Transcripts;

namespace M3.Net.Modules.Transcripts.Application.Transcripts.GetTranscriptRequest;

public sealed record TranscriptRequestResponse(
    Guid Id,
    Guid UserId,
    string YouTubeUrl,
    string? Title,
    string? Description,
    int? DurationSeconds,
    TranscriptRequestStatus Status,
    string? RejectionReason,
    DateTime RequestedAt,
    DateTime? ValidatedAt,
    DateTime? CompletedAt);
