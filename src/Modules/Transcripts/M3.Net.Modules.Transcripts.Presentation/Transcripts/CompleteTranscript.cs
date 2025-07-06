using M3.Net.Common.Domain;
using M3.Net.Common.Presentation.Endpoints;
using M3.Net.Common.Presentation.Results;
using M3.Net.Modules.Transcripts.Application.Transcripts.CompleteTranscript;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace M3.Net.Modules.Transcripts.Presentation.Transcripts;

internal sealed class CompleteTranscript : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("transcripts/complete", async (Request request, ISender sender) =>
        {
            Result<Guid> result = await sender.Send(new CompleteTranscriptCommand(
                request.RequestId,
                request.TranscriptContent,
                request.Language,
                request.ConfidenceScore));

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Transcripts)
        .WithSummary("Complete a transcript")
        .WithDescription("Marks a transcript as completed with the generated content (called by the Python microservice)");
    }

    internal sealed class Request
    {
        public Guid RequestId { get; init; }
        public string TranscriptContent { get; init; } = string.Empty;
        public string Language { get; init; } = string.Empty;
        public decimal ConfidenceScore { get; init; }
    }
}
