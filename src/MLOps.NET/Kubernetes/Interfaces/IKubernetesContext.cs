using MLOps.NET.Entities.Impl;
using System.Threading.Tasks;

namespace MLOps.NET.Kubernetes.Interfaces
{
    /// <summary>
    /// Kubernetes context
    /// </summary>
    public interface IKubernetesContext
    {
        /// <summary>
        /// Creates a namespace in a Kubernetes cluster
        /// </summary>
        /// <param name="experimentName"></param>
        /// <param name="deploymentTarget"></param>
        /// <returns></returns>
        Task<string> CreateNamespaceAsync(string experimentName, DeploymentTarget deploymentTarget);

        /// <summary>
        /// Deploys a container to the configured Kubernetes cluster
        /// </summary>
        /// <param name="experimentName"></param>
        /// <param name="deploymentTarget"></param>
        /// <param name="containerName"></param>
        /// <param name="namespaceName"></param>
        /// <returns></returns>
        Task DeployContainerAsync(string experimentName, DeploymentTarget deploymentTarget, string containerName, string namespaceName);
    }
}
