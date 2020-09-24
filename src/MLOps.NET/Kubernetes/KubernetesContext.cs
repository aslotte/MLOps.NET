using MLOps.NET.Docker.Interfaces;
using MLOps.NET.Docker.Settings;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Kubernetes.Interfaces;
using MLOps.NET.Kubernetes.Settings;
using System.IO;
using System.IO.Abstractions;
using System.Reflection;
using System.Threading.Tasks;

namespace MLOps.NET.Kubernetes
{
    internal sealed class KubernetesContext : IKubernetesContext
    {
        private readonly ICliExecutor cliExecutor;
        private readonly KubernetesSettings kubernetesSettings;
        private readonly DockerSettings dockerSettings;
        private readonly FileSystem fileSystem;
        private readonly string DeployManifestName = "deploy.yml";
        private readonly string ServiceManifestName = "service.yml";

        public KubernetesContext(ICliExecutor cliExecutor, KubernetesSettings kubernetesSettings, DockerSettings dockerSettings, FileSystem fileSystem)
        {
            this.cliExecutor = cliExecutor;
            this.kubernetesSettings = kubernetesSettings;
            this.dockerSettings = dockerSettings;
            this.fileSystem = fileSystem;
        }

        public async Task<string> CreateNamespaceAsync(string experimentName, DeploymentTarget deploymentTarget)
        {
            var name = $"{experimentName}-{deploymentTarget.Name}";

            await cliExecutor.CreateNamespaceAsync(name, kubernetesSettings);
            return name;
        }

        public async Task DeployContainer(string experimentName, DeploymentTarget deploymentTarget, string containerToDeploy, string namespaceName)
        {
            await cliExecutor.CreateImagePullSecret(kubernetesSettings, dockerSettings, namespaceName);

            ParametrizeDeployment(experimentName, containerToDeploy);
            ParametrizeService(experimentName, namespaceName);

            await cliExecutor.KubctlApplyAsync(kubernetesSettings, DeployManifestName);
            await cliExecutor.KubctlApplyAsync(kubernetesSettings, ServiceManifestName);
        }

        private void ParametrizeService(string experimentName, string namespaceName)
        {
            var manifest = ReadResource(ServiceManifestName);
            manifest = string.Format(manifest, experimentName, namespaceName);

            WriteFile(ServiceManifestName, manifest);
        }

        private void ParametrizeDeployment(string experimentName, string imageName)
        {
            var manifest = ReadResource(DeployManifestName);
            manifest = string.Format(manifest, experimentName, imageName);

            WriteFile(DeployManifestName, manifest);
        }

        private void WriteFile(string fileName, string content)
        {
            var basePath = Assembly.GetExecutingAssembly().Location;
            var filePath = Path.Join(basePath, fileName);

            this.fileSystem.File.WriteAllText(filePath, content);
        }

        private string ReadResource(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = string.Join("MLOps.NET", "Manifests", fileName);

            using Stream stream = assembly.GetManifestResourceStream(resourceName);
            using StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
