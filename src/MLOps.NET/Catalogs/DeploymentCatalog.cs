using MLOps.NET.Docker.Interfaces;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Extensions;
using MLOps.NET.Kubernetes.Interfaces;
using MLOps.NET.Services.Interfaces;
using MLOps.NET.Storage;
using MLOps.NET.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MLOps.NET.Catalogs
{
    /// <summary>
    /// Exposes actions for deploying a model
    /// </summary>
    public sealed class DeploymentCatalog
    {
        private readonly IDeploymentRepository deploymentRepository;
        private readonly IModelRepository modelRepository;
        private readonly IExperimentRepository experimentRepository;
        private readonly IRunRepository runRepository;
        private readonly IDockerContext dockerContext;
        private readonly IKubernetesContext kubernetesContext;
        private readonly ISchemaGenerator schemaGenerator;

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="deploymentRepository"></param>
    /// <param name="modelRepository"></param>
    /// <param name="experimentRepository"></param>
    /// <param name="runRepository"></param>
    /// <param name="dockerContext"></param>
    /// <param name="kubernetesContext"></param>
    /// <param name="schemaGenerator"></param>
        public DeploymentCatalog(IDeploymentRepository deploymentRepository,
            IModelRepository modelRepository,
            IExperimentRepository experimentRepository,
            IRunRepository runRepository,
            IDockerContext dockerContext,
            IKubernetesContext kubernetesContext,
            ISchemaGenerator schemaGenerator)
        {
            this.deploymentRepository = deploymentRepository;
            this.modelRepository = modelRepository;
            this.experimentRepository = experimentRepository;
            this.runRepository = runRepository;
            this.dockerContext = dockerContext;
            this.kubernetesContext = kubernetesContext;
            this.schemaGenerator = schemaGenerator;
        }

        /// <summary>
        /// Creates a deployment target
        /// </summary>
        /// <param name="deploymentTargetName"></param>
        /// <param name="isProduction"></param>
        public async Task<DeploymentTarget> CreateDeploymentTargetAsync(string deploymentTargetName, bool isProduction = false)
        {
            return await this.deploymentRepository.CreateDeploymentTargetAsync(deploymentTargetName, isProduction);
        }

        /// <summary>
        /// Get deployment targets
        /// </summary>
        /// <returns></returns>
        public List<DeploymentTarget> GetDeploymentTargets()
        {
            return this.deploymentRepository.GetDeploymentTargets();
        }

        /// <summary>
        /// Deploys a registered model to an URI so that it can be consumed by e.g. an ASP.NET Core Web App
        /// </summary>
        /// <param name="deploymentTarget"></param>
        /// <param name="registeredModel"></param>
        /// <param name="deployedBy"></param>
        /// <returns></returns>
        public async Task<Deployment> DeployModelToUriAsync(DeploymentTarget deploymentTarget, RegisteredModel registeredModel, string deployedBy)
        {
            var experiment = this.experimentRepository.GetExperiment(registeredModel.ExperimentId);
            var deploymentUri = await this.modelRepository.DeployModelAsync(deploymentTarget, registeredModel, experiment);

            return await this.deploymentRepository.CreateDeploymentAsync(deploymentTarget, registeredModel, deployedBy, deploymentUri);
        }

        /// <summary>
        /// Deploys a model to a Docker container in a Kubernetes Cluster using the following steps
        /// - Generates an ASP.NET Core API to serve the model via dynamically generated ModelInput and ModelOutput
        /// - Builds a Docker Image
        /// - Pushes the Docker Image to the registry defined in UseContainerRegistry
        /// - Deploys the Docker Image to a Kubernetes cluster defined in UseKubernetes
        /// </summary>
        /// <param name="deploymentTarget"></param>
        /// <param name="registeredModel"></param>
        /// <param name="deployedBy"></param>
        /// <returns></returns>
        public async Task<Deployment> DeployModelToKubernetesAsync<TModelInput, TModelOutput>(DeploymentTarget deploymentTarget, RegisteredModel registeredModel, string deployedBy)
            where TModelInput : class
            where TModelOutput : class
        {
            await BuildAndPushImageAsync<TModelInput, TModelOutput>(registeredModel);

            var deploymentUri = await DeployContainerToCluster(deploymentTarget, registeredModel);

            return await this.deploymentRepository.CreateDeploymentAsync(deploymentTarget, registeredModel, deployedBy, deploymentUri);
        }

        /// <summary>
        /// - Builds a Docker Image for an ASP.NET Core App using <typeparamref name="TModelInput"/> and <typeparamref name="TModelOutput"/>
        /// - Pushes the Docker Image to the registry defined in UseContainerRegistry
        /// </summary>
        /// <param name="registeredModel"></param>
        /// <returns></returns>
        public async Task BuildAndPushImageAsync<TModelInput, TModelOutput>(RegisteredModel registeredModel) where TModelInput : class
          where TModelOutput : class
        {
            AssertContainerRegistryHasBeenConfigured();

            (string ModelInput, string ModelOutput) GetSchema()
            {
                var modelInput = schemaGenerator.GenerateDefinition<TModelInput>("ModelInput");
                var modelOutput = schemaGenerator.GenerateDefinition<TModelOutput>("ModelOutput");
                return (modelInput, modelOutput);
            }

            var experiment = experimentRepository.GetExperiment(registeredModel.ExperimentId);

            using var model = new MemoryStream();
            await modelRepository.DownloadModelAsync(registeredModel.RunId, model);

            await dockerContext.BuildImage(experiment.ExperimentName, registeredModel, model, GetSchema);
            await dockerContext.PushImage(experiment.ExperimentName, registeredModel);
        }

        /// <summary>
        /// Get the deployment URI
        /// </summary>
        /// <param name="experimentId"></param>
        /// <param name="deploymentTarget"></param>
        /// <returns></returns>
        public string GetDeploymentUri(Guid experimentId, DeploymentTarget deploymentTarget)
        {
            var experiment = this.experimentRepository.GetExperiment(experimentId);

            return this.modelRepository.GetDeploymentUri(experiment, deploymentTarget);
        }

        /// <summary>
        /// Registers the model input and model output schema definition to a run
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="runId"></param>
        /// <returns></returns>
        public async Task RegisterSchema<TInput, TOutput>(Guid runId) where TInput : class where TOutput : class
        {
            var modelInput = schemaGenerator.GenerateDefinition<TInput>("ModelInput");
            var modelOutput = schemaGenerator.GenerateDefinition<TOutput>("ModelOutput");
            var modelInputSchema = new ModelSchema(runId, "ModelInput", modelInput);
            var modelOutputSchema = new ModelSchema(runId, "ModelOutput", modelOutput);
            var schemas = new List<ModelSchema> { modelInputSchema, modelOutputSchema };
            await this.runRepository.RegisterSchema<TInput, TOutput>(runId, schemas);
        }

        /// <summary>
        /// Gets deployments by experiment id
        /// </summary>
        /// <param name="experimentId"></param>
        /// <returns></returns>
        public List<Deployment> GetDeployments(Guid experimentId)
        {
            return this.deploymentRepository.GetDeployments(experimentId);
        }

        private async Task<string> DeployContainerToCluster(DeploymentTarget deploymentTarget, RegisteredModel registeredModel)
        {
            AssertKubernetesClusterHasBeenConfigured();

            var experimentName = experimentRepository.GetExperiment(registeredModel.ExperimentId).ExperimentName;
            var imageName = dockerContext.ComposeImageName(experimentName, registeredModel);

            var namespaceName = await kubernetesContext.CreateNamespaceAsync(experimentName, deploymentTarget);

            return await kubernetesContext.DeployContainerAsync(experimentName, imageName, namespaceName);
        }

        private void AssertContainerRegistryHasBeenConfigured()
        {
            if (dockerContext == null)
            {
                throw new InvalidOperationException($"A container registry has not been configured. Please configure a container registry by calling {nameof(MLOpsBuilderExtensions.UseContainerRegistry)} first");
            }
        }

        private void AssertKubernetesClusterHasBeenConfigured()
        {
            if (kubernetesContext == null)
            {
                throw new InvalidOperationException($"A kubernetes cluster has not been configured. Please configure a kubernetes cluster by calling {nameof(MLOpsBuilderExtensions.UseKubernetes)} first");
            }
        }
    }
}
