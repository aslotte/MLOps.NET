using Microsoft.EntityFrameworkCore;
using MLOps.NET.Storage.Database;
using MLOps.NET.Storage.EntityConfiguration;
using MLOps.NET.Storage.Interfaces;

namespace MLOps.NET.SQLite
{
    /// <summary>
    /// SQLite DbContext
    /// </summary>
    public sealed class MLOpsSQLiteDbContext : MLOpsDbContext
    {
        ///<inheritdoc cref="IMLOpsDbContext"/>
        public MLOpsSQLiteDbContext(DbContextOptions options) : base(options)
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
