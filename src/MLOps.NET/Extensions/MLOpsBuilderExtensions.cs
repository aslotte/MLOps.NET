using Dynamitey;
using MLOps.NET.Docker;
using MLOps.NET.Docker.Settings;
using MLOps.NET.Storage;
using MLOps.NET.Storage.Deployments;
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
            builder.UseModelRepository(new LocalFileModelRepository(new FileSystem(), new ModelPathGenerator(), destinationFolder));

            return builder;
        }

        /// <summary>
        /// Configures a container registry
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="registryName"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static MLOpsBuilder UseContainerRegistry(this MLOpsBuilder builder, string registryName, string username, string password)
        {
            var settings = new DockerSettings
            {
                RegistryName = registryName,
                Password = password,
                Username = username
            };

            var dockerContext = new DockerContext(new CliExecutor(settings), new FileSystem(), settings);
            builder.UseDockerContext(dockerContext);

            return builder;
        }
    }
}
