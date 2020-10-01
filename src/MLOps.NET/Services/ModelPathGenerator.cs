using MLOps.NET.Entities.Impl;
using MLOps.NET.Services.Interfaces;
using System;
using System.IO;

namespace MLOps.NET.Services
{
    ///<inheritdoc cref="IModelPathGenerator"/>
    public sealed class ModelPathGenerator : IModelPathGenerator
    {
        ///<inheritdoc cref="IModelPathGenerator"/>
        public string GetModelName(Guid runId) => $"{runId}.zip";

        ///<inheritdoc cref="IModelPathGenerator"/>
        public string GetDeployedModelName(string experimentName) => $"{experimentName}.zip";

        ///<inheritdoc cref="IModelPathGenerator"/>
        public string GetDeploymentPath(DeploymentTarget deploymentTarget, string experimentName)
        {
            return Path.Combine(experimentName, deploymentTarget.Name, GetDeployedModelName(experimentName));
        }
    }
}
