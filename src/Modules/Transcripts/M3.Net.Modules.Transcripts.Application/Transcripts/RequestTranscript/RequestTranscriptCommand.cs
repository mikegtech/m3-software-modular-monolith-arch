using M3.Net.Common.Application.Messaging;

namespace M3.Net.Modules.Transcripts.Application.Transcripts.RequestTranscript;

public sealed record RequestTranscriptCommand(
    Guid UserId,
    string YouTubeUrl,
    string? Title = null,
    string? Description = null) : ICommand<Guid>;
