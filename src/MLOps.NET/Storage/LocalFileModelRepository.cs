using MLOps.NET.Entities.Impl;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace MLOps.NET.Storage
{
    internal sealed class LocalFileModelRepository : IModelRepository
    {
        private readonly string destinationFolder;
        private readonly string modelRepository;
        private readonly IFileSystem fileSystem;

        public LocalFileModelRepository(IFileSystem fileSystem, string destinationFolder = null)
        {
            if (string.IsNullOrWhiteSpace(destinationFolder))
            {
                destinationFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".mlops");
            }

            this.destinationFolder = destinationFolder;
            this.modelRepository = Path.Combine(destinationFolder, "model-repository");
            this.fileSystem = fileSystem;
        }

        public async Task UploadModelAsync(Guid runId, string sourceFilePath)
        {
            using var fileStream = this.fileSystem.File.OpenRead(sourceFilePath);

            if (!this.fileSystem.Directory.Exists(modelRepository))
                this.fileSystem.Directory.CreateDirectory(modelRepository);

            string destFile = this.fileSystem.Path.Combine(modelRepository, $"{runId}.zip");
            await Task.Run(() => { this.fileSystem.File.Copy(sourceFilePath, destFile, true); });
        }

        public async Task DownloadModelAsync(Guid runId, Stream destination)
        {
            string sourceFile = this.fileSystem.Path.Combine(modelRepository, $"{runId}.zip");
            if (!this.fileSystem.File.Exists(sourceFile))
            {
                throw new FileNotFoundException($"Run artifact {sourceFile} was not found");
            }
            using var fileStream = this.fileSystem.File.OpenRead(sourceFile);
            await fileStream.CopyToAsync(destination);
        }

        public async Task<string> DeployModelAsync(DeploymentTarget deploymentTarget, RegisteredModel registeredModel)
        {
            var deploymentFolder = CreateDeploymentFolder(registeredModel, deploymentTarget);

            var sourceFilePath = this.fileSystem.Path.Combine(modelRepository, $"{registeredModel.RunId}.zip");
            if (!this.fileSystem.File.Exists(sourceFilePath))
            {
                throw new InvalidOperationException("The model to be deployed does not exist");
            }

            var deployedFilePath = this.fileSystem.Path.Combine(deploymentFolder, $"{registeredModel.RunId}.zip");

            await Task.Run(() => this.fileSystem.File.Copy(sourceFilePath, deployedFilePath));

            return deployedFilePath;
        }

        private string CreateDeploymentFolder(RegisteredModel registeredModel, DeploymentTarget deploymentTarget)
        {
            var experimentName = registeredModel.Experiment.ExperimentName;
            var deploymentFolder = this.fileSystem.Path.Combine(destinationFolder, "deployment", experimentName, deploymentTarget.Name);

            if (!this.fileSystem.Directory.Exists(deploymentFolder))
            {
                this.fileSystem.Directory.CreateDirectory(deploymentFolder);
            }
            return deploymentFolder;
        }
    }
}
