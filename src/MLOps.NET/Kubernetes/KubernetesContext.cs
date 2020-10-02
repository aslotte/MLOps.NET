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
        private readonly IManifestParameterizator manifestParameterizator;

        public KubernetesContext(ICliExecutor cliExecutor, KubernetesSettings kubernetesSettings, DockerSettings dockerSettings, IManifestParameterizator manifestParameterizator)
        {
            this.cliExecutor = cliExecutor;
            this.kubernetesSettings = kubernetesSettings;
            this.dockerSettings = dockerSettings;
            this.manifestParameterizator = manifestParameterizator;
        }

        public async Task<string> CreateNamespaceAsync(string experimentName, DeploymentTarget deploymentTarget)
        {
            var namespaceName = $"{experimentName}-{deploymentTarget.Name}".ToLower();

            await cliExecutor.CreateNamespaceAsync(kubernetesSettings, namespaceName);
            return namespaceName;
        }

        public async Task<string> DeployContainerAsync(string experimentName, string imageName, string namespaceName)
        {
            await cliExecutor.CreateImagePullSecret(kubernetesSettings, dockerSettings, namespaceName);

            manifestParameterizator.ParameterizeDeploymentManifest(experimentName, imageName, namespaceName);
            manifestParameterizator.ParameterizeServiceManifest(experimentName, namespaceName);

            await cliExecutor.KubctlApplyAsync(kubernetesSettings, kubernetesSettings.DeployManifestName);
            await cliExecutor.KubctlApplyAsync(kubernetesSettings, kubernetesSettings.ServiceManifestName);

            var externalIP = await cliExecutor.GetServiceExternalIPAsync(kubernetesSettings, experimentName, namespaceName);

            return "http://" + externalIP + "/api/Prediction";
        }
    }
}
