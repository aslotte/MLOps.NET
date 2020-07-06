using Microsoft.EntityFrameworkCore;
using MLOps.NET.Storage;

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

            builder.UseMetaDataStore(new MetaDataStore(new DbContextFactory(options)));

            return builder;
        }
    }
}
