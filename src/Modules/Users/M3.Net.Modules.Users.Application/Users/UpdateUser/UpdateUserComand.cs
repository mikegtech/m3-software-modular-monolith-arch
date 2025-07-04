using M3.Net.Common.Application.Messaging;

namespace M3.Net.Modules.Users.Application.Users.UpdateUser;

public sealed record UpdateUserCommand(Guid UserId, string FirstName, string LastName) : ICommand;
