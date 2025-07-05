using M3.Net.Common.Infrastructure.Inbox;
using M3.Net.Common.Infrastructure.Outbox;
using M3.Net.Modules.Transcripts.Application.Abstractions.Data;
using M3.Net.Modules.Transcripts.Domain.Transcripts;
using M3.Net.Modules.Transcripts.Infrastructure.ProcessingJobs;
using M3.Net.Modules.Transcripts.Infrastructure.TranscriptDeliveries;
using M3.Net.Modules.Transcripts.Infrastructure.TranscriptRequests;
using M3.Net.Modules.Transcripts.Infrastructure.Transcripts;
using Microsoft.EntityFrameworkCore;

namespace M3.Net.Modules.Transcripts.Infrastructure.Database;

public sealed class TranscriptsDbContext(DbContextOptions<TranscriptsDbContext> options) : DbContext(options), IUnitOfWork
{
    internal DbSet<TranscriptRequest> TranscriptRequests { get; set; }
    internal DbSet<ProcessingJob> ProcessingJobs { get; set; }
    internal DbSet<Transcript> Transcripts { get; set; }
    internal DbSet<TranscriptDelivery> TranscriptDeliveries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Transcripts);

        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConsumerConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConsumerConfiguration());

        modelBuilder.ApplyConfiguration(new TranscriptRequestConfiguration());
        modelBuilder.ApplyConfiguration(new ProcessingJobConfiguration());
        modelBuilder.ApplyConfiguration(new TranscriptConfiguration());
        modelBuilder.ApplyConfiguration(new TranscriptDeliveryConfiguration());
    }
}
