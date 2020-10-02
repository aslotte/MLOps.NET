using MLOps.NET.Kubernetes.Interfaces;
using MLOps.NET.Kubernetes.Settings;
using System.IO;
using System.IO.Abstractions;
using System.Reflection;

namespace MLOps.NET.Kubernetes
{
    internal sealed class ManifestParameterizator : IManifestParameterizator
    {
        private readonly IFileSystem fileSystem;
        private readonly KubernetesSettings kubernetesSettings;

        public ManifestParameterizator(IFileSystem fileSystem, KubernetesSettings kubernetesSettings)
        {
            this.fileSystem = fileSystem;
            this.kubernetesSettings = kubernetesSettings;
        }

        public void ParameterizeServiceManifest(string experimentName, string namespaceName)
        {
            var manifest = ReadResource(kubernetesSettings.ServiceManifestName);
            manifest = manifest.Replace(nameof(experimentName), experimentName.ToLower());
            manifest = manifest.Replace(nameof(namespaceName), namespaceName.ToLower());

            WriteFile(kubernetesSettings.ServiceManifestName, manifest);
        }

        public void ParameterizeDeploymentManifest(string experimentName, string imageName, string namespaceName)
        {
            var manifest = ReadResource(kubernetesSettings.DeployManifestName);
            manifest = manifest.Replace(nameof(experimentName), experimentName.ToLower());
            manifest = manifest.Replace(nameof(imageName), imageName.ToLower());
            manifest = manifest.Replace(nameof(namespaceName), namespaceName.ToLower());

            WriteFile(kubernetesSettings.DeployManifestName, manifest);
        }

        private void WriteFile(string fileName, string content)
        {
            var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filePath = Path.Join(basePath, fileName);

            this.fileSystem.File.WriteAllText(filePath, content);
        }

        private string ReadResource(string fileName)
        {
            var assembly = Assembly.GetAssembly(typeof(ManifestParameterizator));
            var resourceName = string.Join(".", assembly.GetName().Name, "Kubernetes.Manifests", fileName);

            using Stream stream = assembly.GetManifestResourceStream(resourceName);
            using StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
