using Microsoft.EntityFrameworkCore;
using MLOps.NET.Storage.Interfaces;
using System;

namespace MLOps.NET.Storage.Database
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
        private readonly Func<IMLOpsDbContext> createDbContext;

        ///<inheritdoc cref="IDbContextFactory"/>
        public DbContextFactory(Func<IMLOpsDbContext> createDbContext)
        {
            this.createDbContext = createDbContext;
        }

        ///<inheritdoc cref="IDbContextFactory"/>
        public IMLOpsDbContext CreateDbContext() => createDbContext();
    }
}
