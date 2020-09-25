using MLOps.NET.Docker.Interfaces;
using MLOps.NET.Docker.Settings;
using MLOps.NET.Entities.Impl;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace MLOps.NET.Docker
{
    ///<inheritdoc cref="IDockerContext"/>
    internal sealed class DockerContext : IDockerContext
    {
        private readonly ICliExecutor cliExecutor;
        private readonly FileSystem fileSystem;
        private readonly DockerSettings dockerSettings;

        public DockerContext(ICliExecutor cliExecutor, FileSystem fileSystem, DockerSettings dockerSettings)
        {
            this.cliExecutor = cliExecutor;
            this.fileSystem = fileSystem;
            this.dockerSettings = dockerSettings;
        }

        ///<inheritdoc cref="IDockerContext"/>
        public async Task BuildImage(string experimentName, RegisteredModel registeredModel, Stream model)
        {
            await cliExecutor.InstallTemplatePackage(dockerSettings);
            await cliExecutor.CreateTemplateProject(dockerSettings);

            await this.CopyModel(model);

            //Todo: Issue #302 (Copy over ModelInput.cs and ModelOutput.cs)

            var imageTag = ComposeImageTag(experimentName, registeredModel);

            await cliExecutor.RunDockerBuild(imageTag, dockerSettings);
        }

        ///<inheritdoc cref="IDockerContext"/>
        public async Task PushImage(string experimentName, RegisteredModel registeredModel)
        {
            var imageTag = ComposeImageTag(experimentName, registeredModel);

            await cliExecutor.RunDockerLogin(dockerSettings);
            await cliExecutor.RunDockerPush(imageTag);
        }

        private async Task CopyModel(Stream model)
        {
            using var fileStream = fileSystem.FileStream.Create($"{dockerSettings.DirectoryName}/{dockerSettings.ModelName}", FileMode.Create, FileAccess.Write);

            await model.CopyToAsync(fileStream);
        }

        public string ComposeImageTag(string experimentName, RegisteredModel registeredModel)
        {
            return $"{dockerSettings.RegistryName}/{experimentName}:{registeredModel.Version}";
        }
    }
}
