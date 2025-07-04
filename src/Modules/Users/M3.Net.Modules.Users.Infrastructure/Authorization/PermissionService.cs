using M3.Net.Common.Application.Authorization;
using M3.Net.Common.Domain;
using M3.Net.Modules.Users.Application.Users.GetUserPermissions;
using MediatR;

namespace M3.Net.Modules.Users.Infrastructure.Authorization;

internal sealed class PermissionService(ISender sender) : IPermissionService
{
    public async Task<Result<PermissionsResponse>> GetUserPermissionsAsync(string identityId)
    {
        return await sender.Send(new GetUserPermissionsQuery(identityId));
    }
}
