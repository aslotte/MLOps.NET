using MLOps.NET.Entities.Impl;
using System;

namespace MLOps.NET.Storage.Deployments
{
    /// <summary>
    /// Model file path generator
    /// </summary>
    public interface IModelPathGenerator
    {
        /// <summary>
        /// Get full model name including extension
        /// </summary>
        /// <param name="runId"></param>
        string GetModelName(Guid runId);

        /// <summary>
        /// Gets the deployed model's name
        /// </summary>
        /// <param name="experimentName"></param>
        /// <returns></returns>
        string GetDeployedModelName(string experimentName);

        /// <summary>
        /// Gets deployment path based on deployment target and the experiment name
        /// </summary>
        /// <param name="deploymentTarget"></param>
        /// <param name="experimentName"></param>
        /// <returns>Path to file</returns>
        string GetDeploymentPath(DeploymentTarget deploymentTarget, string experimentName);
    }
}
