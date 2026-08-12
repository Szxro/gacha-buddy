using Account.Application.Contracts;
using Account.Domain.Entities;
using Account.Infrastructure.Common;
using Account.Infrastructure.Common.Attributes;
using Account.Infrastructure.Common.Enums;
using Account.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Account.Infrastructure.Repositories;

[Inject(ServiceKind.Repository,  serviceLifetime: ServiceLifetime.Scoped)]
public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext dbContext) : base(dbContext) { }
}