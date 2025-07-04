namespace M3.Net.Common.Application.Authorization;

public sealed record PermissionsResponse(Guid UserId, HashSet<string> Permissions);
