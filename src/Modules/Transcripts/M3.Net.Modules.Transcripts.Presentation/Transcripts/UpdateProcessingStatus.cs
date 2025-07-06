using M3.Net.Common.Domain;
using M3.Net.Common.Presentation.Endpoints;
using M3.Net.Common.Presentation.Results;
using M3.Net.Modules.Transcripts.Application.Transcripts.UpdateProcessingStatus;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace M3.Net.Modules.Transcripts.Presentation.Transcripts;

internal sealed class UpdateProcessingStatus : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("transcripts/processing/{requestId:guid}/status", async (Guid requestId, UpdateProcessingStatusRequest request, ISender sender) =>
        {
            Result result = await sender.Send(new UpdateProcessingStatusCommand(
                requestId,
                request.ProgressPercentage,
                request.StatusMessage));

            return result.Match(() => Results.NoContent(), ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Transcripts)
        .WithSummary("Update processing status")
        .WithDescription("Updates the processing status of a transcript generation job (called by the Python microservice)");
    }

    internal sealed class UpdateProcessingStatusRequest
    {
        public int ProgressPercentage { get; init; }
        public string? StatusMessage { get; init; }
    }
}
