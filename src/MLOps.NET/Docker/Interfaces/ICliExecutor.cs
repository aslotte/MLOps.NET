using MLOps.NET.Docker.Settings;
using MLOps.NET.Kubernetes.Settings;
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
        /// <param name="dockerSettings"></param>
        /// <returns></returns>
        Task InstallTemplatePackage(DockerSettings dockerSettings);

        /// <summary>
        /// Uninstall the dotnet new template package for ML.NET.Templates
        /// </summary>
        /// <returns></returns>
        Task UninstallTemplatePackage();

        /// <summary>
        /// Creates a template project
        /// </summary>
        /// <param name="dockerSettings"></param>
        /// <returns></returns>
        Task CreateTemplateProject(DockerSettings dockerSettings);

        /// <summary>
        /// Runs docker build
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="dockerSettings"></param>
        /// <returns></returns>
        Task RunDockerBuild(string tagName, DockerSettings dockerSettings);

        /// <summary>
        /// Runs docker login if username and password have been provided
        /// </summary>
        /// <param name="dockerSettings"></param>
        /// <returns></returns>
        Task RunDockerLogin(DockerSettings dockerSettings);

        /// <summary>
        /// Runs docker push
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        Task RunDockerPush(string tagName);

        /// <summary>
        /// Runs docker pull
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns>True if image exists</returns>
        Task<bool> RunDockerPull(string tagName);

        /// <summary>
        /// Runs docker image rm (remove image)
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        Task RemoveDockerImage(string tagName);

        /// <summary>
        /// Runs kubectl create namespace
        /// </summary>
        /// <param name="name"></param>
        /// <param name="kubernetesSettings"></param>
        /// <returns></returns>
        Task CreateNamespace(string name, KubernetesSettings kubernetesSettings);

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
    }
}
