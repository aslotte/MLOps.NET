using MLOps.NET.Entities.Impl;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MLOps.NET.Docker.Interfaces
{
    /// <summary>
    /// Operations related to Docker
    /// </summary>
    public interface IDockerContext
    {
        /// <summary>
        /// Builds a docker image
        /// </summary>
        /// <param name="experiment"></param>
        /// <param name="registeredModel"></param>
        /// <param name="model"></param>
        /// <param name="GetSchema"></param>
        /// <returns></returns>
        Task BuildImage(Experiment experiment, RegisteredModel registeredModel, Stream model, Func<(string ModelInput, string ModelOutput)> GetSchema);

        /// <summary>
        /// Pushes a docker image to a registry
        /// </summary>
        /// <param name="experimentName"></param>
        /// <param name="registeredModel"></param>
        /// <returns></returns>
        Task PushImage(string experimentName, RegisteredModel registeredModel);

        /// <summary>
        /// Creates an image tag
        /// </summary>
        /// <param name="experimentName"></param>
        /// <param name="registeredModel"></param>
        string ComposeImageName(string experimentName, RegisteredModel registeredModel);
    }
}
