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
        private readonly DbContextOptions options;
        private readonly Action<ModelBuilder> OnModelCreatingAction;

        ///<inheritdoc cref="IDbContextFactory"/>
        public DbContextFactory(DbContextOptions options, Action<ModelBuilder> OnModelCreatingAction)
        {
            this.options = options;
            this.OnModelCreatingAction = OnModelCreatingAction;
        }

        ///<inheritdoc cref="IDbContextFactory"/>
        public IMLOpsDbContext CreateDbContext() => new MLOpsDbContext(this.options, this.OnModelCreatingAction);
    }
}
