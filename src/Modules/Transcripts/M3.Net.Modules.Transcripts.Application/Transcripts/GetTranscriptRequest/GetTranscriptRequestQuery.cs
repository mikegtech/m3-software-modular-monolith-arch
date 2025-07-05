using M3.Net.Common.Application.Messaging;

namespace M3.Net.Modules.Transcripts.Application.Transcripts.GetTranscriptRequest;

public sealed record GetTranscriptRequestQuery(Guid RequestId) : IQuery<TranscriptRequestResponse>;
