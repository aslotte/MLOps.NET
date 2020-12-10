using MLOps.NET.Docker.Settings;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Kubernetes.Settings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MLOps.NET.Docker.Interfaces
{
    /// <summary>
    /// Executes CLI commands
    /// </summary>
    public interface ICliExecutor
    {
        /// <summary>
        /// Installs the dotnet new template package ML.NET.Templates
        /// </summary>
        /// <returns></returns>
        Task InstallTemplatePackage();

        /// <summary>
        /// Adds package dependencies
        /// </summary>
        /// <returns></returns>
        Task AddPackageDependencies(List<PackageDependency> packageDependencies);

        /// <summary>
        /// Uninstall the dotnet new template package for ML.NET.Templates
        /// </summary>
        /// <returns></returns>
        Task UninstallTemplatePackage();

        /// <summary>
        /// Creates a template project
        /// </summary>
        /// <returns></returns>
        Task CreateTemplateProject();

        /// <summary>
        /// Runs docker build
        /// </summary>
        /// <param name="imageName"></param>
        /// <returns></returns>
        Task RunDockerBuild(string imageName);

        /// <summary>
        /// Runs docker login if username and password have been provided
        /// </summary>
        /// <param name="dockerSettings"></param>
        /// <returns></returns>
        Task RunDockerLogin(DockerSettings dockerSettings);

        /// <summary>
        /// Runs docker push
        /// </summary>
        /// <param name="imageName"></param>
        /// <returns></returns>
        Task RunDockerPush(string imageName);

        /// <summary>
        /// Runs docker pull
        /// </summary>
        /// <param name="imageName"></param>
        /// <returns>True if image exists</returns>
        Task<bool> RunDockerPull(string imageName);

        /// <summary>
        /// Runs docker image rm (remove image)
        /// </summary>
        /// <param name="imageName"></param>
        /// <returns></returns>
        Task RemoveDockerImage(string imageName);

        /// <summary>
        /// Runs kubectl create namespace
        /// </summary>
        /// <param name="namespaceName"></param>
        /// <param name="kubernetesSettings"></param>
        /// <returns></returns>
        Task CreateNamespaceAsync(KubernetesSettings kubernetesSettings, string namespaceName);

        /// <summary>
        /// Creates an image pull secret for a Kubernetes namespace
        /// </summary>
        /// <param name="kubernetesSettings"></param>
        /// <param name="dockerSettings"></param>
        /// <param name="namespaceName"></param>
        Task CreateImagePullSecret(KubernetesSettings kubernetesSettings, DockerSettings dockerSettings, string namespaceName);

        /// <summary>
        /// Applies Kubernetes manifest to cluster
        /// </summary>
        /// <param name="kubernetesSettings"></param>
        /// <param name="manifestName"></param>
        /// <returns></returns>
        Task KubctlApplyAsync(KubernetesSettings kubernetesSettings, string manifestName);

        /// <summary>
        /// Gets the external IP address of the exposed Kubernetes service
        /// </summary>
        /// <param name="kubernetesSettings"></param>
        /// <param name="experimentName"></param>
        /// <param name="namespaceName"></param>
        /// <returns></returns>
        Task<string> GetServiceExternalIPAsync(KubernetesSettings kubernetesSettings, string experimentName, string namespaceName);
    }
}
