using M3.Net.Common.Application.Messaging;
using M3.Net.Common.Domain;
using M3.Net.Modules.Transcripts.Application.Abstractions.Data;
using M3.Net.Modules.Transcripts.Domain.Transcripts;

namespace M3.Net.Modules.Transcripts.Application.Transcripts.RequestTranscript;

internal sealed class RequestTranscriptCommandHandler : ICommandHandler<RequestTranscriptCommand, Guid>
{
    private readonly ITranscriptRequestRepository _transcriptRequestRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RequestTranscriptCommandHandler(
        ITranscriptRequestRepository transcriptRequestRepository,
        IUnitOfWork unitOfWork)
    {
        _transcriptRequestRepository = transcriptRequestRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(RequestTranscriptCommand request, CancellationToken cancellationToken)
    {
        // Check for duplicate requests within 24 hours
        TranscriptRequest? existingRequest = await _transcriptRequestRepository
            .GetByUserAndUrlAsync(request.UserId, request.YouTubeUrl, cancellationToken);

        if (existingRequest is not null &&
            existingRequest.RequestedAt > DateTime.UtcNow.AddHours(-24))
        {
            return Result.Failure<Guid>(TranscriptErrors.TranscriptRequest.DuplicateRequest);
        }

        // Create new transcript request
        var transcriptRequest = TranscriptRequest.Create(
            request.UserId,
            request.YouTubeUrl,
            request.Title,
            request.Description);

        _transcriptRequestRepository.Insert(transcriptRequest);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(transcriptRequest.Id);
    }
}
