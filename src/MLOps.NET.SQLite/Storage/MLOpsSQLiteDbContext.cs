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
        /// <summary>
        /// Ctor for migrations
        /// </summary>
        public MLOpsSQLiteDbContext() : base(new DbContextOptionsBuilder()
                .UseSqlite("Data Source=local.db")
                .Options, RelationalEntityConfigurator.OnModelCreating)
        {
        }

        ///<inheritdoc cref="IMLOpsDbContext"/>
        public MLOpsSQLiteDbContext(DbContextOptions options) : base(options, RelationalEntityConfigurator.OnModelCreating)
        {
        }
    }
}
