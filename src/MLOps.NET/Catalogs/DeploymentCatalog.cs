using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage;
using MLOps.NET.Storage.Interfaces;
using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="deploymentRepository"></param>
        /// <param name="modelRepository"></param>
        public DeploymentCatalog(IDeploymentRepository deploymentRepository, IModelRepository modelRepository)
        {
            this.deploymentRepository = deploymentRepository;
            this.modelRepository = modelRepository;
        }

        /// <summary>
        /// Creates a deployment target
        /// </summary>
        /// <param name="deploymentTargetName"></param>
        /// <param name="isProduction"></param>
        public async Task CreateDeploymentTargetAsync(string deploymentTargetName, bool isProduction = false)
        {
            await this.deploymentRepository.CreateDeploymentTargetAsync(deploymentTargetName, isProduction);
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
        /// Deploys a registered model
        /// </summary>
        /// <param name="deploymentTarget"></param>
        /// <param name="registeredModel"></param>
        /// <param name="deployedBy"></param>
        /// <returns>Deployed model path</returns>
        /// <returns></returns>
        public async Task<string> DeployModelAsync(DeploymentTarget deploymentTarget, RegisteredModel registeredModel, string deployedBy)
        {
            await this.deploymentRepository.CreateDeploymentAsync(deploymentTarget, registeredModel, deployedBy);

            return await this.modelRepository.DeployModelAsync(deploymentTarget, registeredModel);
        }

        /// <summary>
        /// Returns the URI to a deployed model
        /// </summary>
        /// <param name="deployment"></param>
        /// <returns></returns>
        public string GetDeploymentUri(Deployment deployment)
        {
            return this.modelRepository.GetDeploymentUri(deployment);
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
    }
}
