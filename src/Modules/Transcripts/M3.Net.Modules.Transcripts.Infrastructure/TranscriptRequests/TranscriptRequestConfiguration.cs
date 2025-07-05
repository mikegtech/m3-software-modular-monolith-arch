using M3.Net.Modules.Transcripts.Domain.Transcripts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace M3.Net.Modules.Transcripts.Infrastructure.TranscriptRequests;

internal sealed class TranscriptRequestConfiguration : IEntityTypeConfiguration<TranscriptRequest>
{
    public void Configure(EntityTypeBuilder<TranscriptRequest> builder)
    {
        builder.HasKey(tr => tr.Id);

        builder.Property(tr => tr.UserId)
            .IsRequired();

        builder.Property(tr => tr.YouTubeUrl)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(tr => tr.Title)
            .HasMaxLength(500);

        builder.Property(tr => tr.Description)
            .HasMaxLength(2000);

        builder.Property(tr => tr.DurationSeconds);

        builder.Property(tr => tr.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(tr => tr.RejectionReason)
            .HasMaxLength(1000);

        builder.Property(tr => tr.RequestedAt)
            .IsRequired();

        builder.Property(tr => tr.ValidatedAt);

        builder.Property(tr => tr.CompletedAt);

        builder.ToTable("transcript_requests");
    }
}
