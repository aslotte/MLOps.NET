using MLOps.NET.Docker;
using MLOps.NET.Docker.Settings;
using MLOps.NET.Storage;
using MLOps.NET.Storage.Deployments;
using System;
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
        /// Configures a container registry with basic authetication
        /// Registry name should e.g. be the following
        /// Docker Hub: docker.io
        /// ACR: yourRegistry.mlopsnet.azurecr.io
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="registryName"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static MLOpsBuilder UseContainerRegistry(this MLOpsBuilder builder, string registryName, string username, string password)
        {
            if (string.IsNullOrEmpty(registryName)) throw new InvalidOperationException($"{nameof(registryName)} cannot be empty");
            if (string.IsNullOrEmpty(username)) throw new InvalidOperationException($"{nameof(username)} cannot be empty");
            if (string.IsNullOrEmpty(password)) throw new InvalidOperationException($"{nameof(password)} cannot be empty");

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

        /// <summary>
        /// Configures a container registry without authentication
        /// Registry name should e.g. be the following
        /// Docker Hub: docker.io
        /// ACR: yourRegistry.mlopsnet.azurecr.io
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="registryName"></param>
        /// <returns></returns>
        public static MLOpsBuilder UseContainerRegistry(this MLOpsBuilder builder, string registryName)
        {
            if (string.IsNullOrEmpty(registryName)) throw new InvalidOperationException($"{nameof(registryName)} cannot be empty");

            var settings = new DockerSettings
            {
                RegistryName = registryName,
            };

            var dockerContext = new DockerContext(new CliExecutor(settings), new FileSystem(), settings);
            builder.UseDockerContext(dockerContext);

            return builder;
        }
    }
}
