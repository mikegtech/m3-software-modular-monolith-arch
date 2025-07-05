using M3.Net.Modules.Transcripts.Domain.Transcripts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace M3.Net.Modules.Transcripts.Infrastructure.TranscriptDeliveries;

internal sealed class TranscriptDeliveryConfiguration : IEntityTypeConfiguration<TranscriptDelivery>
{
    public void Configure(EntityTypeBuilder<TranscriptDelivery> builder)
    {
        builder.HasKey(td => td.Id);

        builder.Property(td => td.Id)
            .IsRequired();

        builder.Property(td => td.TranscriptId)
            .IsRequired();

        builder.Property(td => td.UserId)
            .IsRequired();

        builder.Property(td => td.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(td => td.DeliveryMethod)
            .HasMaxLength(100);

        builder.Property(td => td.AccessToken)
            .HasMaxLength(50);

        builder.Property(td => td.CreatedAt)
            .IsRequired();

        builder.Property(td => td.DeliveredAt);

        builder.Property(td => td.FirstAccessedAt);

        builder.Property(td => td.LastAccessedAt);

        builder.Property(td => td.ExpiresAt);

        builder.Property(td => td.AccessCount)
            .IsRequired();

        // Index for efficient lookup by transcript
        builder.HasIndex(td => td.TranscriptId);

        // Index for efficient lookup by user
        builder.HasIndex(td => td.UserId);

        // Unique index for access token
        builder.HasIndex(td => td.AccessToken)
            .IsUnique();

        builder.ToTable("transcript_deliveries");
    }
}
