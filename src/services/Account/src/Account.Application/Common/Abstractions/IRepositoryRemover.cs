using System.Linq.Expressions;
using Account.Domain.Common;

namespace Account.Application.Common.Abstractions;

public interface IRepositoryRemover<TEntity>
    where TEntity : Entity
{
    void Delete(TEntity entity);

    void DeleteRange(IEnumerable<TEntity> entities);

    Task<int?> DeleteById(int id, CancellationToken cancellationToken = default);

    Task<int?> BulkDelete(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);
}