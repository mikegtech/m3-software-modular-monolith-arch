using M3.Net.Modules.Transcripts.Domain.Transcripts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace M3.Net.Modules.Transcripts.Infrastructure.ProcessingJobs;

internal sealed class ProcessingJobConfiguration : IEntityTypeConfiguration<ProcessingJob>
{
    public void Configure(EntityTypeBuilder<ProcessingJob> builder)
    {
        builder.HasKey(pj => pj.Id);

        builder.Property(pj => pj.TranscriptRequestId)
            .IsRequired();

        builder.Property(pj => pj.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(pj => pj.CreatedAt)
            .IsRequired();

        builder.Property(pj => pj.StartedAt);

        builder.Property(pj => pj.CompletedAt);

        builder.Property(pj => pj.ErrorDetails)
            .HasMaxLength(1000);

        // Index for efficient lookup by transcript request
        builder.HasIndex(pj => pj.TranscriptRequestId);

        builder.ToTable("processing_jobs");
    }
}
