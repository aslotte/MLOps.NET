using MLOps.NET.Entities.Impl;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MLOps.NET.Storage.Interfaces
{
    /// <summary>
    /// Repository to manage deployment related entities
    /// </summary>
    public interface IDeploymentRepository
    {
        /// <summary>
        /// Creates a deployment target
        /// </summary>
        /// <param name="deploymentTargetName"></param>
        Task CreateDeploymentTargetAsync(string deploymentTargetName);

        /// <summary>
        /// Gets all deployment targets
        /// </summary>
        /// <returns></returns>
        List<DeploymentTarget> GetDeploymentTargets();
    }
}
