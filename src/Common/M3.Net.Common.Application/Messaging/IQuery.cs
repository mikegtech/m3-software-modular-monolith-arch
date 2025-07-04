using M3.Net.Common.Domain;
using MediatR;

namespace M3.Net.Common.Application.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
