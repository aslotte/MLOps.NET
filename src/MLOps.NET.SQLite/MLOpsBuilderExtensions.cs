using Microsoft.EntityFrameworkCore;
using MLOps.NET.Storage.Database;

namespace MLOps.NET.SQLite
{
    /// <summary>
    /// Extension methods to allow the usage of SQLite
    /// </summary>
    public static class MLOpsBuilderExtensions
    {
        /// <summary>
        /// Enables the usage of SQLite
        /// </summary>
        /// <param name="builder">MLOpsBuilder for using SQLite</param>
        /// <param name="dbFileLocation">Path to SQLite database file</param>
        /// <returns>Provided MLOpsBuilder for chaining</returns>
        public static MLOpsBuilder UseSQLite(this MLOpsBuilder builder, string dbFileLocation = null)
        {
            var location = dbFileLocation == null ? "local.db" : dbFileLocation;

            var options = new DbContextOptionsBuilder()
                .UseSqlite($"Data Source={location}")
                .Options;

            var contextFactory = new DbContextFactory(() => new MLOpsSQLiteDbContext(options));

            contextFactory.CreateDbContext().EnsureCreated();

            builder.UseMetaDataRepositories(contextFactory);

            return builder;
        }
    }
}
