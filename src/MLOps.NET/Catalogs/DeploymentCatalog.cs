using MLOps.NET.Docker.Interfaces;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Extensions;
using MLOps.NET.Kubernetes.Interfaces;
using MLOps.NET.Kubernetes.Settings;
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
        private readonly IDockerContext dockerContext;
        private readonly IKubernetesContext kubernetesContext;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="deploymentRepository"></param>
        /// <param name="modelRepository"></param>
        /// <param name="experimentRepository"></param>
        /// <param name="dockerContext"></param>
        /// <param name="kubernetesContext"></param>
        public DeploymentCatalog(IDeploymentRepository deploymentRepository,
            IModelRepository modelRepository,
            IExperimentRepository experimentRepository,
            IDockerContext dockerContext,
            IKubernetesContext kubernetesContext)
        {
            this.deploymentRepository = deploymentRepository;
            this.modelRepository = modelRepository;
            this.experimentRepository = experimentRepository;
            this.dockerContext = dockerContext;
            this.kubernetesContext = kubernetesContext;
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
        /// Deploys a registered model to a URI
        /// </summary>
        /// <param name="deploymentTarget"></param>
        /// <param name="registeredModel"></param>
        /// <param name="deployedBy"></param>
        /// <returns>A deployment</returns>
        /// <returns></returns>
        public async Task<Deployment> DeployModelAsync(DeploymentTarget deploymentTarget, RegisteredModel registeredModel, string deployedBy)
        {
            var experiment = this.experimentRepository.GetExperiment(registeredModel.ExperimentId);
            var deploymentUri = await this.modelRepository.DeployModelAsync(deploymentTarget, registeredModel, experiment);

            return await this.deploymentRepository.CreateDeploymentAsync(deploymentTarget, registeredModel, deployedBy, deploymentUri);
        }

        /// <summary>
        /// Deploys a model to a container in a cluster by
        /// - Building an ASP.NET Core Web App to wrap around the ML.NET Model
        /// - Builds a Docker Image based on the project
        /// - Pushes the image to the registry defined in UseContainerRegistry
        /// - Deploys the model to a cluster defined in UseKubernetes
        /// </summary>
        /// <param name="deploymentTarget"></param>
        /// <param name="registeredModel"></param>
        /// <param name="deployedBy"></param>
        /// <returns></returns>
        public async Task<Deployment> DeployModelToContainerAsync(DeploymentTarget deploymentTarget, RegisteredModel registeredModel, string deployedBy)
        {
            await BuildAndPushImageAsync(registeredModel);

            await DeployContainerToCluster(deploymentTarget, registeredModel);

            //TODO: Get the service url and set that in the deployment

            return await this.deploymentRepository.CreateDeploymentAsync(deploymentTarget, registeredModel, deployedBy, deploymentUri: "");
        }

        /// <summary>
        /// Deploys a container to a Kubernetes cluster configured in the MLOpsBuilder.
        /// The container will be deployed into the 
        /// namespace {experimentName}-{deploymentTargetName}
        /// </summary>
        /// <param name="deploymentTarget"></param>
        /// <param name="registeredModel"></param>
        /// <returns></returns>
        private async Task DeployContainerToCluster(DeploymentTarget deploymentTarget, RegisteredModel registeredModel)
        {
            AssertKubernetesClusterHasBeenConfigured();

            var experimentName = experimentRepository.GetExperiment(registeredModel.ExperimentId).ExperimentName;
            var imageName = dockerContext.ComposeImageTag(experimentName, registeredModel);

            var namespaceName = await kubernetesContext.CreateNamespaceAsync(experimentName, deploymentTarget);

            await kubernetesContext.DeployContainerAsync(experimentName, deploymentTarget, imageName, namespaceName);
        }

        /// <summary>
        /// Builds a docker image for an ASP.NET Core Web App with an ML.NET model
        /// embedded and pushes it to the registry defined in UseContainerRegistry
        /// </summary>
        /// <param name="registeredModel"></param>
        /// <returns></returns>
        public async Task BuildAndPushImageAsync(RegisteredModel registeredModel)
        {
            AssertContainerRegistryHasBeenConfigured();

            var experiment = experimentRepository.GetExperiment(registeredModel.ExperimentId);
            using var model = new MemoryStream();

            await dockerContext.BuildImage(experiment.ExperimentName, registeredModel, model);
            await dockerContext.PushImage(experiment.ExperimentName, registeredModel);
        }

        /// <summary>
        /// Returns the URI to a deployed model
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
        /// Gets deployments by experiment id
        /// </summary>
        /// <param name="experimentId"></param>
        /// <returns></returns>
        public List<Deployment> GetDeployments(Guid experimentId)
        {
            return this.deploymentRepository.GetDeployments(experimentId);
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
