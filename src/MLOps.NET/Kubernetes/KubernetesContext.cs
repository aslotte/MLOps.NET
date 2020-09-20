using MLOps.NET.Docker.Interfaces;
using MLOps.NET.Docker.Settings;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Kubernetes.Interfaces;
using MLOps.NET.Kubernetes.Settings;
using System.Threading.Tasks;

namespace MLOps.NET.Kubernetes
{
    internal sealed class KubernetesContext : IKubernetesContext
    {
        private readonly ICliExecutor cliExecutor;
        private readonly KubernetesSettings kubernetesSettings;
        private readonly DockerSettings dockerSettings;

        public KubernetesContext(ICliExecutor cliExecutor, KubernetesSettings kubernetesSettings, DockerSettings dockerSettings)
        {
            this.cliExecutor = cliExecutor;
            this.kubernetesSettings = kubernetesSettings;
            this.dockerSettings = dockerSettings;
        }
        public async Task<string> CreateNamespace(string experimentName, DeploymentTarget deploymentTarget)
        {
            var name = $"{experimentName}-{deploymentTarget.Name}";

            await cliExecutor.CreateNamespace(name, kubernetesSettings);
            return name;
        }

        public async Task DeployContainer(string experimentName, DeploymentTarget deploymentTarget, string containerToDeploy, string namespaceName)
        {
            await cliExecutor.CreateImagePullSecret(kubernetesSettings, dockerSettings, namespaceName);

            //Replace values in deployment.yml
            //Replace values in service.yml

            //Execute commands to apply both
        }
    }
}
