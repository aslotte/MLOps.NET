using Microsoft.EntityFrameworkCore;
using MLOps.NET.Azure.Storage;
using MLOps.NET.Storage.Database;
using MLOps.NET.Storage.Interfaces;

namespace MLOps.NET.Azure
{
    /// <summary>
    /// SQL Server DbContext
    /// </summary>
    public sealed class MLOpsCosmosDbContext : MLOpsDbContext
    {
        ///<inheritdoc cref="IMLOpsDbContext"/>
        public MLOpsCosmosDbContext(DbContextOptions options) : base(options)
        {
        }

        ///<inheritdoc cref="IMLOpsDbContext"/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            CosmosEntityConfigurator.OnModelCreating(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }
    }
}
