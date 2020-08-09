using Microsoft.EntityFrameworkCore;
using MLOps.NET.Storage.Database;
using MLOps.NET.Storage.EntityConfiguration;
using MLOps.NET.Storage.Interfaces;

namespace MLOps.NET.SQLServer
{
    /// <summary>
    /// SQL Server DbContext
    /// </summary>
    public sealed class MLOpsSQLDbContext : MLOpsDbContext
    {
        ///<inheritdoc cref="IMLOpsDbContext"/>
        public MLOpsSQLDbContext(DbContextOptions options) : base(options)
        {
        }

        ///<inheritdoc cref="IMLOpsDbContext"/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            RelationalEntityConfigurator.OnModelCreating(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }
    }
}
