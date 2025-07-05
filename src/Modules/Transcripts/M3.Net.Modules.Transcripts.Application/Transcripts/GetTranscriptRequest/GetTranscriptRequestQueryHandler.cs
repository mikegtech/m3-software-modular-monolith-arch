using M3.Net.Common.Application.Messaging;
using M3.Net.Common.Domain;
using M3.Net.Modules.Transcripts.Domain.Transcripts;

namespace M3.Net.Modules.Transcripts.Application.Transcripts.GetTranscriptRequest;

internal sealed class GetTranscriptRequestQueryHandler : IQueryHandler<GetTranscriptRequestQuery, TranscriptRequestResponse>
{
    private readonly ITranscriptRequestRepository _transcriptRequestRepository;

    public GetTranscriptRequestQueryHandler(ITranscriptRequestRepository transcriptRequestRepository)
    {
        _transcriptRequestRepository = transcriptRequestRepository;
    }

    public async Task<Result<TranscriptRequestResponse>> Handle(
        GetTranscriptRequestQuery request,
        CancellationToken cancellationToken)
    {
        TranscriptRequest? transcriptRequest = await _transcriptRequestRepository
            .GetAsync(request.RequestId, cancellationToken);

        if (transcriptRequest is null)
        {
            return Result.Failure<TranscriptRequestResponse>(
                TranscriptErrors.TranscriptRequest.NotFound);
        }

        var response = new TranscriptRequestResponse(
            transcriptRequest.Id,
            transcriptRequest.UserId,
            transcriptRequest.YouTubeUrl,
            transcriptRequest.Title,
            transcriptRequest.Description,
            transcriptRequest.DurationSeconds,
            transcriptRequest.Status,
            transcriptRequest.RejectionReason,
            transcriptRequest.RequestedAt,
            transcriptRequest.ValidatedAt,
            transcriptRequest.CompletedAt);

        return Result.Success(response);
    }
}
