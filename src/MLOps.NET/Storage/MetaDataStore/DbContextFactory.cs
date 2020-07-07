using Microsoft.EntityFrameworkCore;
using MLOps.NET.Storage.Interfaces;

namespace MLOps.NET.Storage
{
    /// <summary>
    /// Factory to create Db Context
    /// </summary>
    public interface IDbContextFactory
    {
        /// <summary>
        /// Creates a DbContext based on name
        /// </summary>
        /// <returns></returns>
        IMLOpsDbContext CreateDbContext();
    }

    ///<inheritdoc cref="IDbContextFactory"/>
    public sealed class DbContextFactory : IDbContextFactory
    {
        private readonly DbContextOptions options;

        ///<inheritdoc cref="IDbContextFactory"/>
        public DbContextFactory(DbContextOptions options)
        {
            this.options = options;
        }

        ///<inheritdoc cref="IDbContextFactory"/>
        public IMLOpsDbContext CreateDbContext() => new MLOpsDbContext(this.options);
    }
}
