using M3.Net.Common.Application.Messaging;
using M3.Net.Common.Domain;
using M3.Net.Modules.Transcripts.Application.Transcripts.GetTranscriptRequest;
using M3.Net.Modules.Transcripts.Domain.Transcripts;

namespace M3.Net.Modules.Transcripts.Application.Transcripts.GetUserTranscriptRequests;

internal sealed class GetUserTranscriptRequestsQueryHandler
    : IQueryHandler<GetUserTranscriptRequestsQuery, IEnumerable<TranscriptRequestResponse>>
{
    private readonly ITranscriptRequestRepository _transcriptRequestRepository;

    public GetUserTranscriptRequestsQueryHandler(ITranscriptRequestRepository transcriptRequestRepository)
    {
        _transcriptRequestRepository = transcriptRequestRepository;
    }

    public async Task<Result<IEnumerable<TranscriptRequestResponse>>> Handle(
        GetUserTranscriptRequestsQuery request,
        CancellationToken cancellationToken)
    {
        IEnumerable<TranscriptRequest> transcriptRequests = await _transcriptRequestRepository
            .GetByUserAsync(request.UserId, cancellationToken);

        var responses = transcriptRequests
            .OrderByDescending(tr => tr.RequestedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(tr => new TranscriptRequestResponse(
                tr.Id,
                tr.UserId,
                tr.YouTubeUrl,
                tr.Title,
                tr.Description,
                tr.DurationSeconds,
                tr.Status,
                tr.RejectionReason,
                tr.RequestedAt,
                tr.ValidatedAt,
                tr.CompletedAt))
            .ToList();

        return Result.Success<IEnumerable<TranscriptRequestResponse>>(responses);
    }
}
