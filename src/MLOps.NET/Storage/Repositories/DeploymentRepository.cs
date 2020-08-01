using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage.Database;
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

        ///<inheritdoc cref="IDeploymentRepository"/>
        public DeploymentRepository(IDbContextFactory contextFactory, IClock clock)
        {
            this.contextFactory = contextFactory;
            this.clock = clock;
        }

        ///<inheritdoc cref="IDeploymentRepository"/>
        public async Task CreateDeploymentTargetAsync(string deploymentTargetName)
        {
            if (string.IsNullOrEmpty(deploymentTargetName))
            {
                throw new ArgumentNullException("Deployment target name was not specified");
            }

            using var db = this.contextFactory.CreateDbContext();

            var deploymentTarget = new DeploymentTarget(deploymentTargetName)
            {
                CreatedDate = clock.UtcNow
            };

            db.DeploymentTargets.Add(deploymentTarget);

            await db.SaveChangesAsync();
        }

        ///<inheritdoc cref="IDeploymentRepository"/>
        public List<DeploymentTarget> GetDeploymentTargets()
        {
            using var db = this.contextFactory.CreateDbContext();

            return db.DeploymentTargets.ToList();
        }
    }
}
