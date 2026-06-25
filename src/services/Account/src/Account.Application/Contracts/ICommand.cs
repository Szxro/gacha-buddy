using Account.SharedKernel.Common.Primitives;
using MediatR;

namespace Account.SharedKernel.Contracts;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>, IBaseCommand { }

public interface ICommand : IRequest<Result>, IBaseCommand { }

public interface IBaseCommand { }