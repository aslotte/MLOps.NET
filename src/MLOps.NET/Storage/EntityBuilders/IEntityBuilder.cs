using Microsoft.EntityFrameworkCore.ChangeTracking;
using MLOps.NET.Storage.Interfaces;

namespace MLOps.NET.Storage.EntityBuilders
{
    /// <summary>
    /// Entity Builder
    /// </summary>
    public interface IEntityBuilder<TEntity> where TEntity : class
    {
        /// <summary>
        /// Build entity
        /// </summary>
        /// <param name="db"></param>
        /// <param name="entity"></param>
        TEntity BuildEntity(IMLOpsDbContext db, TEntity entity);
    }
}
