using MLOps.NET.Docker;
using MLOps.NET.Docker.Settings;
using MLOps.NET.Kubernetes;
using MLOps.NET.Kubernetes.Interfaces;
using MLOps.NET.Kubernetes.Settings;
using MLOps.NET.Services;
using MLOps.NET.Storage;
using System;
using System.IO;
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

            var dockerContext = new DockerContext(new CliExecutor(), new FileSystem(), settings);
            builder.UseDockerContext(dockerContext, settings);

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

            var dockerContext = new DockerContext(new CliExecutor(), new FileSystem(), settings);
            builder.UseDockerContext(dockerContext, settings);

            return builder;
        }

        /// <summary>
        /// Configures to use Kubernetes with the full path to the kubeconfig or the kubeconfig content
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="kubeconfigPathOrContent"></param>
        /// <returns></returns>
        public static MLOpsBuilder UseKubernetes(this MLOpsBuilder builder, string kubeconfigPathOrContent)
        {
            if (string.IsNullOrEmpty(kubeconfigPathOrContent)) throw new InvalidOperationException($"{nameof(kubeconfigPathOrContent)} cannot be empty");

            var settings = new KubernetesSettings
            {
                KubeConfigPath = SetKubeConfig(kubeconfigPathOrContent)
            };

            IKubernetesContext CreateKubernetesContext(DockerSettings dockerSettings)
            {
                return new KubernetesContext(new CliExecutor(), settings, dockerSettings, new ManifestParameterizator(new FileSystem(), settings));
            };

            builder.UseKubernetesContext(CreateKubernetesContext);

            return builder;
        }

        /// <summary>
        /// It is possible that the Kubeconfig can be passed in as either a path to the file
        /// or as the content it self, in which place we need to store it to a file first
        /// </summary>
        /// <param name="kubeconfigPathOrContent"></param>
        /// <returns></returns>
        private static string SetKubeConfig(string kubeconfigPathOrContent)
        {
            if (Path.IsPathFullyQualified(kubeconfigPathOrContent)) return kubeconfigPathOrContent;

            var kubeConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "kubeconfig");
            File.WriteAllText(kubeConfigPath, kubeconfigPathOrContent);

            return kubeConfigPath;
        }
    }
}
