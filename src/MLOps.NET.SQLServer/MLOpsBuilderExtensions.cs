using Microsoft.EntityFrameworkCore;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage;
using MLOps.NET.Storage.EntityConfiguration;

namespace MLOps.NET.SQLServer
{
    /// <summary>
    /// Extension methods to allow the usage of SQL Server
    /// </summary>
    public static class MLOpsBuilderExtensions
    {
        /// <summary>
        /// Enables the usage of SQL Server
        /// </summary>
        /// <param name="builder">MLOpsBuilder for using SQL Server</param>
        /// <param name="connectionString"></param>
        /// <returns>Provided MLOpsBuilder for chaining</returns>
        public static MLOpsBuilder UseSQLServer(this MLOpsBuilder builder, string connectionString)
        {
            var options = new DbContextOptionsBuilder()
                .UseSqlServer(connectionString)
                .Options;

            var contextFactory = new DbContextFactory(options, RelationalEntityConfigurator.OnModelCreating);

            contextFactory.CreateDbContext().EnsureCreated();

            builder.UseMetaDataStore(new MetaDataStore(contextFactory));

            return builder;
        }
    }
}
