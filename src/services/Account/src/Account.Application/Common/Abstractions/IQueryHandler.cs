using Account.SharedKernel.Common.Primitives;
using MediatR;

namespace Account.Application.Common.Abstractions;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{ }