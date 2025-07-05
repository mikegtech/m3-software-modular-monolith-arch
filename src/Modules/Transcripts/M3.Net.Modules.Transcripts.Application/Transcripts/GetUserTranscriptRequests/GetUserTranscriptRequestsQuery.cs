using M3.Net.Common.Application.Messaging;
using M3.Net.Modules.Transcripts.Application.Transcripts.GetTranscriptRequest;

namespace M3.Net.Modules.Transcripts.Application.Transcripts.GetUserTranscriptRequests;

public sealed record GetUserTranscriptRequestsQuery(
    Guid UserId,
    int Page = 1,
    int PageSize = 10) : IQuery<IEnumerable<TranscriptRequestResponse>>;
