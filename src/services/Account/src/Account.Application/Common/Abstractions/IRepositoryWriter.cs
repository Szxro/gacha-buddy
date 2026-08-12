using Account.Domain.Common;

namespace Account.Application.Common.Abstractions;

public interface IRepositoryWriter<TEntity>
    where TEntity : Entity
{
    void Add(TEntity entity);

    void AddRange(IEnumerable<TEntity> entities);

    void Update(TEntity entity);

    void UpdateRange(IEnumerable<TEntity> entities);
}