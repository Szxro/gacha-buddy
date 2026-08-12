using System.Linq.Expressions;
using Account.Domain.Common;

namespace Account.Application.Common.Abstractions;

public interface IRepositoryReader<TEntity>
    where TEntity : Entity
{
    Task<TEntity?> GetById(int id, CancellationToken cancellationToken = default);

    Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);
}