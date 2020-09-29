using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage.Deployments;
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
        private readonly IModelPathGenerator modelPathGenerator;

        public LocalFileModelRepository(IFileSystem fileSystem, 
            IModelPathGenerator modelPathGenerator, 
            string destinationFolder = null)
        {
            if (string.IsNullOrWhiteSpace(destinationFolder))
            {
                destinationFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".mlops");
            }

            this.destinationFolder = destinationFolder;
            this.modelRepository = Path.Combine(destinationFolder, "model-repository");
            this.fileSystem = fileSystem;
            this.modelPathGenerator = modelPathGenerator;
        }

        public async Task UploadModelAsync(Guid runId, string sourceFilePath)
        {
            using var fileStream = this.fileSystem.File.OpenRead(sourceFilePath);

            if (!this.fileSystem.Directory.Exists(modelRepository))
            {
                this.fileSystem.Directory.CreateDirectory(modelRepository);
            }

            string destFile = this.fileSystem.Path.Combine(modelRepository, this.modelPathGenerator.GetModelName(runId));
            await Task.Run(() => { this.fileSystem.File.Copy(sourceFilePath, destFile, true); });
        }

        public async Task DownloadModelAsync(Guid runId, Stream destination)
        {
            string sourceFile = this.fileSystem.Path.Combine(modelRepository, this.modelPathGenerator.GetModelName(runId));

            if (!this.fileSystem.File.Exists(sourceFile))
            {
                throw new FileNotFoundException($"Run artifact {sourceFile} was not found");
            }
            using var fileStream = this.fileSystem.File.OpenRead(sourceFile);
            await fileStream.CopyToAsync(destination);

            destination.Position = 0;
        }

        public async Task<string> DeployModelAsync(DeploymentTarget deploymentTarget, RegisteredModel registeredModel, Experiment experiment)
        {
            var deploymentUri = GetDeploymentUri(experiment, deploymentTarget);

            CreateDeploymentFolder(deploymentUri);

            var modelName = this.modelPathGenerator.GetModelName(registeredModel.RunId);

            var sourceFilePath = this.fileSystem.Path.Combine(modelRepository, modelName);
            if (!this.fileSystem.File.Exists(sourceFilePath))
            {
                throw new InvalidOperationException("The model to be deployed does not exist");
            }

            await Task.Run(() => this.fileSystem.File.Copy(sourceFilePath, deploymentUri, overwrite: true));
            return deploymentUri;
        }

        public string GetDeploymentUri(Experiment experiment, DeploymentTarget deploymentTarget)
        {
            var deploymentPath = this.modelPathGenerator.GetDeploymentPath(deploymentTarget, experiment.ExperimentName);

            return this.fileSystem.Path.Combine(destinationFolder, "deployment", deploymentPath);
        }

        private void CreateDeploymentFolder(string deploymentPath)
        {
            var directory = Directory.GetParent(deploymentPath).FullName;

            if (!this.fileSystem.Directory.Exists(directory))
            {
                this.fileSystem.Directory.CreateDirectory(directory);
            }
        }
    }
}
