using M3.Net.Common.Domain;
using M3.Net.Common.Presentation.Endpoints;
using M3.Net.Common.Presentation.Results;
using M3.Net.Modules.Transcripts.Application.Transcripts.GetTranscriptRequest;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace M3.Net.Modules.Transcripts.Presentation.Transcripts;

internal sealed class GetTranscriptRequest : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("transcripts/request/{id:guid}", async (Guid id, ISender sender) =>
        {
            Result<TranscriptRequestResponse> result = await sender.Send(new GetTranscriptRequestQuery(id));

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Transcripts)
        .WithSummary("Get a transcript request by ID")
        .WithDescription("Retrieves a transcript request by its unique identifier");
    }
}
