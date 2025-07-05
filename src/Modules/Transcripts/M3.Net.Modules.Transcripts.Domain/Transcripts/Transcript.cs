using M3.Net.Common.Domain;

namespace M3.Net.Modules.Transcripts.Domain.Transcripts;

public sealed class Transcript : Entity
{
    private Transcript()
    {
    }

    public Guid Id { get; private set; }

    public Guid RequestId { get; private set; }

    public Guid UserId { get; private set; }

    public string YouTubeUrl { get; private set; } = string.Empty;

    public string Title { get; private set; } = string.Empty;

    public string? Content { get; private set; }

    public string? Language { get; private set; }

    public decimal? ConfidenceScore { get; private set; }

    public int WordCount { get; private set; }

    public int DurationSeconds { get; private set; }

    public TranscriptStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? CompletedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    public static Transcript Create(
        Guid requestId,
        Guid userId,
        string youTubeUrl,
        string title,
        int durationSeconds)
    {
        var transcript = new Transcript
        {
            Id = Guid.NewGuid(),
            RequestId = requestId,
            UserId = userId,
            YouTubeUrl = youTubeUrl,
            Title = title,
            DurationSeconds = durationSeconds,
            Status = TranscriptStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            WordCount = 0
        };

        return transcript;
    }

    public void UpdateContent(
        string content,
        string language,
        decimal confidenceScore)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("Content cannot be empty", nameof(content));
        }

        if (confidenceScore < 0 || confidenceScore > 1)
        {
            throw new ArgumentException("Confidence score must be between 0 and 1", nameof(confidenceScore));
        }

        Content = content;
        Language = language;
        ConfidenceScore = confidenceScore;
        WordCount = CountWords(content);
        Status = TranscriptStatus.Processing;
        UpdatedAt = DateTime.UtcNow;

        Raise(new TranscriptContentReceivedDomainEvent(
            Id,
            RequestId,
            UserId,
            content.Length,
            WordCount,
            language,
            confidenceScore));
    }

    public void MarkAsCompleted()
    {
        if (Status != TranscriptStatus.Processing)
        {
            throw new InvalidOperationException("Cannot complete transcript that is not processing");
        }

        if (string.IsNullOrWhiteSpace(Content))
        {
            throw new InvalidOperationException("Cannot complete transcript without content");
        }

        Status = TranscriptStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        Raise(new TranscriptCompletedDomainEvent(
            Id,
            RequestId,
            UserId,
            CompletedAt.Value));
    }

    public void MarkAsFailed()
    {
        Status = TranscriptStatus.Failed;
        CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        Raise(new TranscriptFailedDomainEvent(
            Id,
            RequestId,
            UserId,
            CompletedAt.Value));
    }

    public void Delete()
    {
        Status = TranscriptStatus.Deleted;
        UpdatedAt = DateTime.UtcNow;

        Raise(new TranscriptDeletedDomainEvent(
            Id,
            RequestId,
            UserId));
    }

    public void UpdateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title cannot be empty", nameof(title));
        }

        Title = title;
        UpdatedAt = DateTime.UtcNow;
    }

    private static int CountWords(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return 0;
        }

        return text.Split(SeparatorChars, StringSplitOptions.RemoveEmptyEntries).Length;
    }

    private static readonly char[] SeparatorChars = [' ', '\t', '\n', '\r'];
}
