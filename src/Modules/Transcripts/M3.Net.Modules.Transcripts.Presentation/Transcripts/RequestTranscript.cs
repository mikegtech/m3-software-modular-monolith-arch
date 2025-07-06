using M3.Net.Common.Domain;
using M3.Net.Common.Presentation.Endpoints;
using M3.Net.Common.Presentation.Results;
using M3.Net.Modules.Transcripts.Application.Transcripts.RequestTranscript;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace M3.Net.Modules.Transcripts.Presentation.Transcripts;

internal sealed class RequestTranscript : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("transcripts/request", async (Request request, ISender sender) =>
        {
            Result<Guid> result = await sender.Send(new RequestTranscriptCommand(
                request.UserId,
                request.YouTubeUrl));

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Transcripts)
        .WithSummary("Request a new transcript for a YouTube video")
        .WithDescription("Submits a request to generate a transcript for the specified YouTube video URL");
    }

    internal sealed class Request
    {
        public Guid UserId { get; init; }
        public string YouTubeUrl { get; init; } = string.Empty;
    }
}
