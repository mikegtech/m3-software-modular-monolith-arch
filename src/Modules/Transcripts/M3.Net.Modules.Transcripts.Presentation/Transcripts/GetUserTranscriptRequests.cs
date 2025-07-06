using M3.Net.Common.Domain;
using M3.Net.Common.Presentation.Endpoints;
using M3.Net.Common.Presentation.Results;
using M3.Net.Modules.Transcripts.Application.Transcripts.GetTranscriptRequest;
using M3.Net.Modules.Transcripts.Application.Transcripts.GetUserTranscriptRequests;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace M3.Net.Modules.Transcripts.Presentation.Transcripts;

internal sealed class GetUserTranscriptRequests : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("transcripts/user/{userId:guid}/requests", async (Guid userId, int page, int pageSize, ISender sender) =>
        {
            Result<IEnumerable<TranscriptRequestResponse>> result = await sender.Send(new GetUserTranscriptRequestsQuery(userId, page, pageSize));

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Transcripts)
        .WithSummary("Get transcript requests for a user")
        .WithDescription("Retrieves all transcript requests made by the specified user");
    }
}
