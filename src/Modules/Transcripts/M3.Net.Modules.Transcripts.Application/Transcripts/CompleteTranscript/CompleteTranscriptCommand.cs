using M3.Net.Common.Application.Messaging;

namespace M3.Net.Modules.Transcripts.Application.Transcripts.CompleteTranscript;

public sealed record CompleteTranscriptCommand(
    Guid RequestId,
    string TranscriptContent,
    string Language,
    decimal ConfidenceScore) : ICommand<Guid>;
