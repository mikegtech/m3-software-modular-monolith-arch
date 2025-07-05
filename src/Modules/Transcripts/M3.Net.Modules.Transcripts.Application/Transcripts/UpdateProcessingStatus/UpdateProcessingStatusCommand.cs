using M3.Net.Common.Application.Messaging;

namespace M3.Net.Modules.Transcripts.Application.Transcripts.UpdateProcessingStatus;

public sealed record UpdateProcessingStatusCommand(
    Guid RequestId,
    int ProgressPercentage,
    string? StatusMessage = null) : ICommand;
