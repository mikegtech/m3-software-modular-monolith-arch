using FluentValidation;

namespace M3.Net.Modules.Transcripts.Application.Transcripts.UpdateProcessingStatus;

internal sealed class UpdateProcessingStatusCommandValidator : AbstractValidator<UpdateProcessingStatusCommand>
{
    public UpdateProcessingStatusCommandValidator()
    {
        RuleFor(x => x.RequestId)
            .NotEmpty()
            .WithMessage("Request ID is required");

        RuleFor(x => x.ProgressPercentage)
            .InclusiveBetween(0, 100)
            .WithMessage("Progress percentage must be between 0 and 100");

        RuleFor(x => x.StatusMessage)
            .MaximumLength(500)
            .WithMessage("Status message cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.StatusMessage));
    }
}
