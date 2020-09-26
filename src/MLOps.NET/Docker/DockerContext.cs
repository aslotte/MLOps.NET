using MLOps.NET.Docker.Interfaces;
using MLOps.NET.Docker.Settings;
using MLOps.NET.Entities.Impl;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace MLOps.NET.Docker
{
    ///<inheritdoc cref="IDockerContext"/>
    internal sealed class DockerContext : IDockerContext
    {
        private readonly ICliExecutor cliExecutor;
        private readonly IFileSystem fileSystem;
        private readonly DockerSettings dockerSettings;

        public DockerContext(ICliExecutor cliExecutor, IFileSystem fileSystem, DockerSettings dockerSettings)
        {
            this.cliExecutor = cliExecutor;
            this.fileSystem = fileSystem;
            this.dockerSettings = dockerSettings;
        }

        ///<inheritdoc cref="IDockerContext"/>
        public async Task BuildImage(string experimentName, RegisteredModel registeredModel, Stream model, Func<(string ModelInput, string ModelOutput)> GetSchema)
        {
            await cliExecutor.InstallTemplatePackage(dockerSettings);
            await cliExecutor.CreateTemplateProject(dockerSettings);

            await this.CopyModel(model);
            await this.CopySchema(GetSchema);

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

        private async Task CopySchema(Func<(string ModelInput, string ModelOutput)> GetSchema)
        {
            var (modelInput, modelOutput) = GetSchema();

            await fileSystem.File.WriteAllTextAsync($"{dockerSettings.DirectoryName}/{"ModelInput.cs"}", modelInput);
            await fileSystem.File.WriteAllTextAsync($"{dockerSettings.DirectoryName}/{"ModelOutput.cs"}", modelOutput);
        }

        public string ComposeImageTag(string experimentName, RegisteredModel registeredModel)
        {
            var imageName = experimentName.Replace(" ", string.Empty).Trim();
            return $"{dockerSettings.RegistryName}/{imageName}:{registeredModel.Version}".ToLower();
        }
    }
}
