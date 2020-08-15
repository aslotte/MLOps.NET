using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage.Database;
using MLOps.NET.Storage.EntityResolvers;
using MLOps.NET.Storage.Interfaces;
using MLOps.NET.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MLOps.NET.Storage.Repositories
{
    ///<inheritdoc cref="IDeploymentRepository"/>
    public sealed class DeploymentRepository : IDeploymentRepository
    {
        private readonly IDbContextFactory contextFactory;
        private readonly IClock clock;
        private readonly IEntityResolver<DeploymentTarget> deploymentTargetResolver;

        ///<inheritdoc cref="IDeploymentRepository"/>
        public DeploymentRepository(IDbContextFactory contextFactory, IClock clock, 
            IEntityResolver<DeploymentTarget> deploymentTargetResolver)
        {
            this.contextFactory = contextFactory;
            this.clock = clock;
            this.deploymentTargetResolver = deploymentTargetResolver;
        }

        ///<inheritdoc cref="IDeploymentRepository"/>
        public async Task<DeploymentTarget> CreateDeploymentTargetAsync(string deploymentTargetName, bool isProduction = false)
        {
            if (string.IsNullOrEmpty(deploymentTargetName))
            {
                throw new ArgumentNullException("Deployment target name was not specified");
            }

            using var db = this.contextFactory.CreateDbContext();

            var existingDeploymentTarget = db.DeploymentTargets.FirstOrDefault(x => x.Name == deploymentTargetName);
            if (existingDeploymentTarget != null)
            {
                return this.deploymentTargetResolver.BuildEntity(db, existingDeploymentTarget);
            }

            var deploymentTarget = new DeploymentTarget(deploymentTargetName)
            {
                CreatedDate = clock.UtcNow,
                IsProduction = isProduction
            };

            db.DeploymentTargets.Add(deploymentTarget);

            await db.SaveChangesAsync();

            return deploymentTarget;
        }

        ///<inheritdoc cref="IDeploymentRepository"/>
        public List<DeploymentTarget> GetDeploymentTargets()
        {
            using var db = this.contextFactory.CreateDbContext();

            var deploymentTargets = db.DeploymentTargets.ToList();

            return this.deploymentTargetResolver.BuildEntities(db, deploymentTargets);
        }

        ///<inheritdoc cref="IDeploymentRepository"/>
        public async Task<Deployment> CreateDeploymentAsync(DeploymentTarget deploymentTarget, RegisteredModel registeredModel, string deployedBy, string deploymentUri)
        {
            using var db = this.contextFactory.CreateDbContext();

            var deployment = new Deployment()
            {
                DeploymentDate = this.clock.UtcNow,
                DeployedBy = deployedBy,
                DeploymentTargetId = deploymentTarget.DeploymentTargetId,
                RegisteredModelId = registeredModel.RegisteredModelId,
                DeploymentUri = deploymentUri
            };

            db.Deployments.Add(deployment);
            await db.SaveChangesAsync();

            return deployment;
        }

        ///<inheritdoc cref="IDeploymentRepository"/>
        public List<Deployment> GetDeployments(Guid experimentId)
        {
            using var db = this.contextFactory.CreateDbContext();

            var registeredModels = db.RegisteredModels
                .Where(x => x.ExperimentId == experimentId)
                .Select(x => x.RegisteredModelId)
                .ToList();

            return db.Deployments
                .Where(x => registeredModels.Contains(x.RegisteredModelId))
                .ToList();
        }
    }
}
