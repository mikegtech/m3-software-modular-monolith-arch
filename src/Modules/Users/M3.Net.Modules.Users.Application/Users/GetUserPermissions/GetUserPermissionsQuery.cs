using M3.Net.Common.Application.Authorization;
using M3.Net.Common.Application.Messaging;

namespace M3.Net.Modules.Users.Application.Users.GetUserPermissions;

public sealed record GetUserPermissionsQuery(string IdentityId) : IQuery<PermissionsResponse>;
