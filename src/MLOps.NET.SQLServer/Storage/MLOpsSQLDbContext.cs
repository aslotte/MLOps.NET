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
        private static readonly string MigrationConnectionString = @"Server=localhost,1433;Database=MLOpsNET_IntegrationTests;User Id=sa;Password=MLOps4TheWin!;";

        /// <summary>
        /// Ctor for migrations
        /// </summary>
        public MLOpsSQLDbContext() : base(new DbContextOptionsBuilder()
                .UseSqlServer(MigrationConnectionString)
                .Options, RelationalEntityConfigurator.OnModelCreating)
        {
        }

        ///<inheritdoc cref="IMLOpsDbContext"/>
        public MLOpsSQLDbContext(DbContextOptions options) : base(options, RelationalEntityConfigurator.OnModelCreating)
        {
        }
    }
}
