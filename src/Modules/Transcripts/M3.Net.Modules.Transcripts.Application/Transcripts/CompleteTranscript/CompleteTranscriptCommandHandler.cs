using M3.Net.Common.Application.Messaging;
using M3.Net.Common.Domain;
using M3.Net.Modules.Transcripts.Application.Abstractions.Data;
using M3.Net.Modules.Transcripts.Domain.Transcripts;

namespace M3.Net.Modules.Transcripts.Application.Transcripts.CompleteTranscript;

internal sealed class CompleteTranscriptCommandHandler : ICommandHandler<CompleteTranscriptCommand, Guid>
{
    private readonly ITranscriptRequestRepository _transcriptRequestRepository;
    private readonly ITranscriptRepository _transcriptRepository;
    private readonly IProcessingJobRepository _processingJobRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CompleteTranscriptCommandHandler(
        ITranscriptRequestRepository transcriptRequestRepository,
        ITranscriptRepository transcriptRepository,
        IProcessingJobRepository processingJobRepository,
        IUnitOfWork unitOfWork)
    {
        _transcriptRequestRepository = transcriptRequestRepository;
        _transcriptRepository = transcriptRepository;
        _processingJobRepository = processingJobRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CompleteTranscriptCommand request, CancellationToken cancellationToken)
    {
        // Get the transcript request
        TranscriptRequest? transcriptRequest = await _transcriptRequestRepository
            .GetAsync(request.RequestId, cancellationToken);

        if (transcriptRequest is null)
        {
            return Result.Failure<Guid>(TranscriptErrors.TranscriptRequest.NotFound);
        }

        // Get or create transcript
        Transcript? transcript = await _transcriptRepository
            .GetByRequestIdAsync(request.RequestId, cancellationToken);

        if (transcript is null)
        {
            transcript = Transcript.Create(
                transcriptRequest.Id,
                transcriptRequest.UserId,
                transcriptRequest.YouTubeUrl,
                transcriptRequest.Title ?? "Untitled",
                transcriptRequest.DurationSeconds ?? 0);

            _transcriptRepository.Insert(transcript);
        }

        // Update transcript with content
        transcript.UpdateContent(
            request.TranscriptContent,
            request.Language,
            request.ConfidenceScore);

        transcript.MarkAsCompleted();

        // Update processing job
        ProcessingJob? processingJob = await _processingJobRepository
            .GetByTranscriptRequestAsync(request.RequestId, cancellationToken);

        if (processingJob is not null)
        {
            processingJob.Complete();
            _processingJobRepository.Update(processingJob);
        }

        // Update request status
        transcriptRequest.MarkAsCompleted();
        _transcriptRequestRepository.Update(transcriptRequest);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(transcript.Id);
    }
}
