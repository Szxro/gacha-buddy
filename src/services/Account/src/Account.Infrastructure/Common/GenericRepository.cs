using System.Linq.Expressions;
using Account.Domain.Common;
using Account.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Account.Infrastructure.Common;

public abstract class GenericRepository<TEntity>
    where TEntity : Entity 
{
    protected readonly AppDbContext DbContext;

    public GenericRepository(AppDbContext dbContext)
    {
        DbContext = dbContext;
    }
    
    public void Add(TEntity entity)
    {
        DbContext.Set<TEntity>().Add(entity);
    }

    public void AddRange(IEnumerable<TEntity> entities)
    {
        DbContext.Set<TEntity>().AddRange(entities);
    }

    public void Delete(TEntity entity)
    {
        DbContext.Set<TEntity>().Remove(entity);
    }

    public void DeleteRange(IEnumerable<TEntity> entities)
    {
        DbContext.Set<TEntity>().RemoveRange(entities);
    }

    public void Update(TEntity entity)
    {
        DbContext.Set<TEntity>().Update(entity);
    }

    public void UpdateRange(IEnumerable<TEntity> entities)
    {
        DbContext.Set<TEntity>().UpdateRange(entities);
    }

    public async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<TEntity>().ToListAsync(cancellationToken);
    }

    public async Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> filter,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query =  DbContext.Set<TEntity>().AsQueryable();
        
        return await query.AnyAsync(filter, cancellationToken);
    }

    public async Task<TEntity?> GetById(int id, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<TEntity>().Where(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int?> DeleteById(int id, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<TEntity>().Where(x => x.Id == id).ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<int?> BulkDelete(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<TEntity>().Where(filter).ExecuteDeleteAsync(cancellationToken);
    }
}