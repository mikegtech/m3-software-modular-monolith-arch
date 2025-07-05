using M3.Net.Modules.Transcripts.Domain.Transcripts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace M3.Net.Modules.Transcripts.Infrastructure.Transcripts;

internal sealed class TranscriptConfiguration : IEntityTypeConfiguration<Transcript>
{
    public void Configure(EntityTypeBuilder<Transcript> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.RequestId)
            .IsRequired();

        builder.Property(t => t.UserId)
            .IsRequired();

        builder.Property(t => t.YouTubeUrl)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(t => t.Content)
            .HasColumnType("text");

        builder.Property(t => t.Language)
            .HasMaxLength(10);

        builder.Property(t => t.ConfidenceScore)
            .HasPrecision(5, 4); // e.g., 0.9876

        builder.Property(t => t.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.CompletedAt);

        builder.Property(t => t.UpdatedAt);

        // Index for efficient lookup by request
        builder.HasIndex(t => t.RequestId);
        builder.HasIndex(t => t.UserId);

        builder.ToTable("transcripts");
    }
}
