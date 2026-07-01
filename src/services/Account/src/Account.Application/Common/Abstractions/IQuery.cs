using Account.SharedKernel.Common.Primitives;
using MediatR;

namespace Account.Application.Common.Abstractions;

public interface IQuery<TResponse> : IRequest<Result<TResponse>> { }