using Account.Application.Common.Abstractions;
using Account.Domain.Entities;

namespace Account.Application.Contracts;

public interface IUserRepository : IRepositoryWriter<User>, IRepositoryReader<User> { }