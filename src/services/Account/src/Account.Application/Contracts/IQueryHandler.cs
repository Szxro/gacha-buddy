using Account.SharedKernel.Common.Primitives;
using MediatR;

namespace Account.SharedKernel.Contracts;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{ }