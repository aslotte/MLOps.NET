using MLOps.NET.Entities.Impl;
using System;

namespace MLOps.NET.Storage.Deployments
{
    ///<inheritdoc cref="IModelPathGenerator"/>
    public sealed class ModelPathGenerator : IModelPathGenerator
    {
        ///<inheritdoc cref="IModelPathGenerator"/>
        public string GetModelName(Guid runId) => $"{runId}.zip";

        ///<inheritdoc cref="IModelPathGenerator"/>
        public string GetDeploymentPath(DeploymentTarget deploymentTarget, RegisteredModel registeredModel)
        {
            var experimentName = registeredModel.Experiment.ExperimentName;

            return string.Join("/", experimentName, deploymentTarget.Name, GetModelName(registeredModel.RunId));
        }
    }
}
