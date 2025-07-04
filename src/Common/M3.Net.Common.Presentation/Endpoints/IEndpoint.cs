using Microsoft.AspNetCore.Routing;

namespace M3.Net.Common.Presentation.Endpoints;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
