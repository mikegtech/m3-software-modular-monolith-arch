namespace M3.Net.Modules.Transcripts.Domain.Transcripts;

public interface ITranscriptRequestRepository
{
    Task<TranscriptRequest?> GetAsync(Guid id, CancellationToken cancellationToken = default);

    Task<TranscriptRequest?> GetByUserAndUrlAsync(Guid userId, string youTubeUrl, CancellationToken cancellationToken = default);

    Task<IEnumerable<TranscriptRequest>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<IEnumerable<TranscriptRequest>> GetPendingValidationAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<TranscriptRequest>> GetQueuedForProcessingAsync(CancellationToken cancellationToken = default);

    void Insert(TranscriptRequest transcriptRequest);

    void Update(TranscriptRequest transcriptRequest);
}

public interface IProcessingJobRepository
{
    Task<ProcessingJob?> GetAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ProcessingJob?> GetByTranscriptRequestAsync(Guid transcriptRequestId, CancellationToken cancellationToken = default);

    Task<IEnumerable<ProcessingJob>> GetActiveJobsAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<ProcessingJob>> GetTimedOutJobsAsync(TimeSpan timeout, CancellationToken cancellationToken = default);

    void Insert(ProcessingJob processingJob);

    void Update(ProcessingJob processingJob);
}

public interface ITranscriptRepository
{
    Task<Transcript?> GetAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Transcript?> GetByRequestIdAsync(Guid requestId, CancellationToken cancellationToken = default);

    Task<IEnumerable<Transcript>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<IEnumerable<Transcript>> GetCompletedTranscriptsAsync(CancellationToken cancellationToken = default);

    void Insert(Transcript transcript);

    void Update(Transcript transcript);
}

public interface ITranscriptDeliveryRepository
{
    Task<TranscriptDelivery?> GetAsync(Guid id, CancellationToken cancellationToken = default);

    Task<TranscriptDelivery?> GetByTranscriptIdAsync(Guid transcriptId, CancellationToken cancellationToken = default);

    Task<TranscriptDelivery?> GetByAccessTokenAsync(string accessToken, CancellationToken cancellationToken = default);

    Task<IEnumerable<TranscriptDelivery>> GetExpiredDeliveriesAsync(CancellationToken cancellationToken = default);

    void Insert(TranscriptDelivery transcriptDelivery);

    void Update(TranscriptDelivery transcriptDelivery);
}
