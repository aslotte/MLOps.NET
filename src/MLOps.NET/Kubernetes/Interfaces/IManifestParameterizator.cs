using System;
using System.Collections.Generic;
using System.Text;

namespace MLOps.NET.Kubernetes.Interfaces
{
    /// <summary>
    /// Service to parameterize kubernetes manifests (yml files)
    /// </summary>
    public interface IManifestParameterizator
    {
        /// <summary>
        /// Parameterizes the service manifest
        /// </summary>
        /// <param name="experimentName"></param>
        /// <param name="namespaceName"></param>
        void ParameterizeServiceManifest(string experimentName, string namespaceName);

        /// <summary>
        /// Parameterizes the deploy manifest
        /// </summary>
        /// <param name="experimentName"></param>
        /// <param name="imageName"></param>
        /// <param name="namespaceName"></param>
        void ParameterizeDeploymentManifest(string experimentName, string imageName, string namespaceName);
    }
}
