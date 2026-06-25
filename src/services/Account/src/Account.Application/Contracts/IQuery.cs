using Account.SharedKernel.Common.Primitives;
using MediatR;

namespace Account.SharedKernel.Contracts;

public interface IQuery<TResponse> : IRequest<Result<TResponse>> { }