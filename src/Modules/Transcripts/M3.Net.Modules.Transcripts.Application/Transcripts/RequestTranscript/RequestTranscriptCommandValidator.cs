using FluentValidation;

namespace M3.Net.Modules.Transcripts.Application.Transcripts.RequestTranscript;

internal sealed class RequestTranscriptCommandValidator : AbstractValidator<RequestTranscriptCommand>
{
    public RequestTranscriptCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.YouTubeUrl)
            .NotEmpty()
            .WithMessage("YouTube URL is required")
            .Must(BeValidYouTubeUrl)
            .WithMessage("Invalid YouTube URL format");

        RuleFor(x => x.Title)
            .MaximumLength(200)
            .WithMessage("Title cannot exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.Title));

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Description cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }

    private static bool BeValidYouTubeUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return false;
        }

        // Basic YouTube URL validation
        string[] validPatterns =
        [
            @"^https?://(www\.)?youtube\.com/watch\?v=[\w\-_]+",
            @"^https?://(www\.)?youtu\.be/[\w\-_]+",
            @"^https?://(www\.)?youtube\.com/embed/[\w\-_]+",
            @"^https?://(www\.)?youtube\.com/v/[\w\-_]+"
        ];

        return validPatterns.Any(pattern =>
            System.Text.RegularExpressions.Regex.IsMatch(url, pattern,
                System.Text.RegularExpressions.RegexOptions.IgnoreCase));
    }
}
