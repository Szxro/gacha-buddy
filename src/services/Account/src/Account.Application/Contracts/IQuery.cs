using Account.SharedKernel.Common.Primitives;
using MediatR;

namespace Account.Application.Contracts;

public interface IQuery<TResponse> : IRequest<Result<TResponse>> { }