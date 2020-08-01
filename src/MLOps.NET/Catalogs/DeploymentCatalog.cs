using MLOps.NET.Entities.Impl;
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

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="deploymentRepository"></param>
        public DeploymentCatalog(IDeploymentRepository deploymentRepository)
        {
            this.deploymentRepository = deploymentRepository;
        }

        /// <summary>
        /// Creates a deployment target
        /// </summary>
        /// <param name="deploymentTargetName"></param>
        public async Task CreateDeploymentTargetAsync(string deploymentTargetName)
        {
            await this.deploymentRepository.CreateDeploymentTargetAsync(deploymentTargetName);
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
        /// <returns></returns>
        public async Task DeployModelAsync(DeploymentTarget deploymentTarget, RegisteredModel registeredModel, string deployedBy)
        {
            await this.deploymentRepository.CreateDeploymentAsync(deploymentTarget, registeredModel, deployedBy);

            //deploy model by copying from model repo to deployment container
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
