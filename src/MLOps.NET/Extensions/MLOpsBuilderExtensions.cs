using MLOps.NET.Storage;
using System.IO.Abstractions;

namespace MLOps.NET.Extensions
{
    /// <summary>
    /// Extensions methods that allow for usage of local file storage for models
    /// </summary>
    public static class MLOpsBuilderExtensions
    {
        /// <summary>
        /// Enables usage of local file share for model storage
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="destinationFolder">Destination folder, default location is .mlops under the current user</param>
        /// <returns>Provided MLOpsBuilder for chaining</returns>
        public static MLOpsBuilder UseLocalFileModelRepository(this MLOpsBuilder builder, string destinationFolder = null)
        {
            builder.UseModelRepository(new LocalFileModelRepository(new FileSystem(), destinationFolder));

            return builder;
        }
    }
}
