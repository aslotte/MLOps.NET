using MLOps.NET.Docker.Interfaces;
using MLOps.NET.Docker.Settings;
using MLOps.NET.Entities.Impl;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
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
        public async Task BuildImage(Experiment experiment, RegisteredModel registeredModel, Stream model, Func<(string ModelInput, string ModelOutput)> GetSchema)
        {
            ResetImageDirectory();

            var packageDependencies = experiment.Runs
                .First(x => x.RunId == registeredModel.RunId)
                .PackageDepedencies;

            await cliExecutor.InstallTemplatePackage(dockerSettings);
            await cliExecutor.CreateTemplateProject(dockerSettings);
            await cliExecutor.AddPackageDependencies(dockerSettings, packageDependencies);

            await this.CopyModel(model);
            await this.CopySchema(GetSchema);


            var imageName = ComposeImageName(experiment.ExperimentName, registeredModel);

            await cliExecutor.RunDockerBuild(dockerSettings, imageName);
        }

        ///<inheritdoc cref="IDockerContext"/>
        public async Task PushImage(string experimentName, RegisteredModel registeredModel)
        {
            var imageTag = ComposeImageName(experimentName, registeredModel);

            await cliExecutor.RunDockerLogin(dockerSettings);
            await cliExecutor.RunDockerPush(imageTag);
        }

        private void ResetImageDirectory()
        {
            if (fileSystem.Directory.Exists(dockerSettings.DirectoryName))
            {
                fileSystem.Directory.Delete(dockerSettings.DirectoryName, recursive: true);
            }
            fileSystem.Directory.CreateDirectory(dockerSettings.DirectoryName);
        }

        private async Task CopyModel(Stream model)
        {
            var path = $"{dockerSettings.DirectoryName}/{dockerSettings.ModelName}";
            using var fileStream = fileSystem.FileStream.Create(path, FileMode.Create, FileAccess.Write);

            await model.CopyToAsync(fileStream);
        }

        private async Task CopySchema(Func<(string ModelInput, string ModelOutput)> GetSchema)
        {
            var (modelInput, modelOutput) = GetSchema();

            var directoryPath = $"{dockerSettings.DirectoryName}/Schema";
            fileSystem.Directory.CreateDirectory(directoryPath);

            await fileSystem.File.WriteAllTextAsync($"{directoryPath}/{"ModelInput.cs"}", modelInput);
            await fileSystem.File.WriteAllTextAsync($"{directoryPath}/{"ModelOutput.cs"}", modelOutput);
        }

        public string ComposeImageName(string experimentName, RegisteredModel registeredModel)
        {
            var imageName = experimentName.Replace(" ", string.Empty).Trim();
            return $"{dockerSettings.RegistryName}/{imageName}:{registeredModel.Version}".ToLower();
        }
    }
}
