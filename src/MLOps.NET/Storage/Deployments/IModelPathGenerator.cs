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
        /// Gets deployment path based on deployment target and the registered model
        /// </summary>
        /// <param name="deploymentTarget"></param>
        /// <param name="registeredModel"></param>
        /// <returns>Path to file</returns>
        string GetDeploymentPath(DeploymentTarget deploymentTarget, RegisteredModel registeredModel);
    }
}
