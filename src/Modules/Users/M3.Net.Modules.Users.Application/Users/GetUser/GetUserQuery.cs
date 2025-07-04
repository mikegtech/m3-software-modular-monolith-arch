using M3.Net.Common.Application.Messaging;

namespace M3.Net.Modules.Users.Application.Users.GetUser;

public sealed record GetUserQuery(Guid UserId) : IQuery<UserResponse>;
