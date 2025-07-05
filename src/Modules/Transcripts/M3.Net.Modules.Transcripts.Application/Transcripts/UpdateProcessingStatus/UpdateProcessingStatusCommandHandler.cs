using M3.Net.Common.Application.Messaging;
using M3.Net.Common.Domain;
using M3.Net.Modules.Transcripts.Application.Abstractions.Data;
using M3.Net.Modules.Transcripts.Domain.Transcripts;

namespace M3.Net.Modules.Transcripts.Application.Transcripts.UpdateProcessingStatus;

internal sealed class UpdateProcessingStatusCommandHandler : ICommandHandler<UpdateProcessingStatusCommand>
{
    private readonly IProcessingJobRepository _processingJobRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProcessingStatusCommandHandler(
        IProcessingJobRepository processingJobRepository,
        IUnitOfWork unitOfWork)
    {
        _processingJobRepository = processingJobRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateProcessingStatusCommand request, CancellationToken cancellationToken)
    {
        ProcessingJob? processingJob = await _processingJobRepository
            .GetByTranscriptRequestAsync(request.RequestId, cancellationToken);

        if (processingJob is null)
        {
            return Result.Failure(TranscriptErrors.ProcessingJob.NotFound);
        }

        try
        {
            processingJob.UpdateProgress(request.ProgressPercentage);

            _processingJobRepository.Update(processingJob);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(Error.Problem(
                "UpdateProcessingStatus.InvalidOperation",
                ex.Message));
        }
    }
}
