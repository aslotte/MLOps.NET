using MLOps.NET.Entities.Impl;
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
        /// <param name="experimentName"></param>
        /// <param name="registeredModel"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task BuildImage(string experimentName, RegisteredModel registeredModel, Stream model);

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
        string ComposeImageTag(string experimentName, RegisteredModel registeredModel);
    }
}
