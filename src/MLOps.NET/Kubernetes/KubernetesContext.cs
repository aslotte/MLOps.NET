using MLOps.NET.Docker.Interfaces;
using MLOps.NET.Docker.Settings;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Kubernetes.Interfaces;
using MLOps.NET.Kubernetes.Settings;
using System.IO;
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
            var name = $"{experimentName}-{deploymentTarget.Name}".ToLower();

            await cliExecutor.CreateNamespaceAsync(name, kubernetesSettings);
            return name;
        }

        public async Task<string> DeployContainerAsync(string experimentName, string containerToDeploy, string namespaceName)
        {
            await cliExecutor.CreateImagePullSecret(kubernetesSettings, dockerSettings, namespaceName);

            manifestParameterizator.ParameterizeDeploymentManifest(experimentName, containerToDeploy, namespaceName);
            manifestParameterizator.ParameterizeServiceManifest(experimentName, namespaceName);

            await cliExecutor.KubctlApplyAsync(kubernetesSettings, kubernetesSettings.DeployManifestName);
            await cliExecutor.KubctlApplyAsync(kubernetesSettings, kubernetesSettings.ServiceManifestName);

            var externalIP = await cliExecutor.GetServiceExternalIPAsync(kubernetesSettings, experimentName, namespaceName);

            return "http://" + externalIP + "/api/Prediction";
        }
    }
}
