using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;

namespace Account.Infrastructure.Extensions;

public static partial class InfrastructureExtensions
{
    public static ModelBuilder ApplyGlobalQueryFilter<TEntity>(
        this ModelBuilder builder,
        Expression<Func<TEntity, bool>> filter)
    {
        // Entities that are assignable to the TEntity generic
        List<IMutableEntityType> entities = builder.Model.GetEntityTypes()
            .Where(entity => typeof(TEntity).IsAssignableFrom(entity.ClrType))
            .ToList();

        foreach (IMutableEntityType entity in entities)
        {
            // Creating a ParameterExpression with the actual type of the current entity
            ParameterExpression entityParam = Expression.Parameter(entity.ClrType, "entity");

            // Replacing the parameter with the actual entity parameter / (e.g => (entityParameter => ....))
            Expression filterBody = ReplacingExpressionVisitor.Replace(filter.Parameters[0], entityParam, filter.Body);

            // Getting the current query filters of the actual entity
            LambdaExpression? existingFilter = entity.GetQueryFilter();

            if (existingFilter is not null)
            {
                // Other filter already present, combine them
                filterBody = ReplacingExpressionVisitor.Replace(entityParam, existingFilter.Parameters[0], filterBody);
                filterBody = Expression.AndAlso(existingFilter.Body, filterBody);

                // Create a new lambda expression for the combined filter
                existingFilter = Expression.Lambda(filterBody, existingFilter.Parameters);
            }
            else
            {
                // Create a new lambda expression for the current filter
                existingFilter = Expression.Lambda(filterBody, entityParam);
            }

            // Setting the query filter to the entity
            entity.SetQueryFilter(existingFilter);
        }

        return builder;
    }
}