using FluentValidation;

namespace M3.Net.Modules.Transcripts.Application.Transcripts.CompleteTranscript;

internal sealed class CompleteTranscriptCommandValidator : AbstractValidator<CompleteTranscriptCommand>
{
    public CompleteTranscriptCommandValidator()
    {
        RuleFor(x => x.RequestId)
            .NotEmpty()
            .WithMessage("Request ID is required");

        RuleFor(x => x.TranscriptContent)
            .NotEmpty()
            .WithMessage("Transcript content is required")
            .MinimumLength(10)
            .WithMessage("Transcript content must be at least 10 characters")
            .MaximumLength(100000)
            .WithMessage("Transcript content cannot exceed 100,000 characters");

        RuleFor(x => x.Language)
            .NotEmpty()
            .WithMessage("Language is required")
            .Length(2, 10)
            .WithMessage("Language must be between 2 and 10 characters");

        RuleFor(x => x.ConfidenceScore)
            .InclusiveBetween(0.0m, 1.0m)
            .WithMessage("Confidence score must be between 0.0 and 1.0");
    }
}
