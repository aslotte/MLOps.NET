using MLOps.NET.Storage.Interfaces;
using System.Collections.Generic;

namespace MLOps.NET.Storage.EntityResolvers
{
    /// <summary>
    /// Entity Builder
    /// </summary>
    public interface IEntityResolver<TEntity> where TEntity : class
    {
        /// <summary>
        /// Resolve entity
        /// </summary>
        /// <param name="db"></param>
        /// <param name="entity"></param>
        TEntity BuildEntity(IMLOpsDbContext db, TEntity entity);

        /// <summary>
        /// Resolve entities
        /// </summary>
        /// <param name="db"></param>
        /// <param name="entities"></param>
        /// <returns></returns>
        List<TEntity> BuildEntities(IMLOpsDbContext db, List<TEntity> entities);
    }
}
