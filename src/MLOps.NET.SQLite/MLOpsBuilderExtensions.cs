using MLOps.NET.Storage;
using System.IO.Abstractions;

namespace MLOps.NET.SQLite
{
    /// <summary>
    /// Extension methods to allow the usage of SQLite
    /// </summary>
    public static class MLOpsBuilderExtensions
    {
        /// <summary>
        /// Enables the usage of SQLite and local storage
        /// </summary>
        /// <param name="builder">MLOpsBuilder to add Azure Storage providers to</param>
        /// <param name="destinationFolder">Destination folder (optional with default value of C:\MLops)</param>
        /// <returns>Provided MLOpsBuilder for chaining</returns>
        public static MLOpsBuilder UseSQLite(this MLOpsBuilder builder, string destinationFolder = null)
        {
            builder.UseMetaDataStore(new SQLiteMetaDataStore());
            builder.UseModelRepository(new LocalFileModelRepository(new FileSystem(), destinationFolder));

            return builder;
        }
    }
}
