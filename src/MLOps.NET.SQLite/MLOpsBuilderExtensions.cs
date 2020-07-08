using Microsoft.EntityFrameworkCore;
using MLOps.NET.Storage;

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
        /// <returns>Provided MLOpsBuilder for chaining</returns>
        public static MLOpsBuilder UseSQLite(this MLOpsBuilder builder)
        {
            var options = new DbContextOptionsBuilder()
                .UseSqlite("Data Source=local.db")
                .Options;

            var contextFactory = new DbContextFactory(options);
            contextFactory.CreateDbContext().EnsureCreated();

            builder.UseMetaDataStore(new MetaDataStore(contextFactory));

            return builder;
        }
    }
}
