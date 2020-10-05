using MLOps.NET.Entities.Impl;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MLOps.NET.Storage
{
    /// <summary>
    /// Repository to access and manage runs
    /// </summary>
    public interface IRunRepository
    {
        /// <summary>
        /// Creates a unqiue run for a given experiment
        /// </summary>
        /// <param name="experimentId"></param>
        /// <param name="packageDepedencies"></param>
        /// <param name="gitCommitHash">Optional, sets the linked git commit hash</param>
        /// <returns>Created run</returns>
        /// <returns></returns>
        Task<Run> CreateRunAsync(Guid experimentId, List<PackageDependency> packageDepedencies, string gitCommitHash = "");

        /// <summary>
        /// Adds model schemas to a Run
        /// </summary>
        /// <param name="runId"></param>
        /// <param name="modelSchemas"></param>
        /// <returns></returns>
        Task CreateModelSchemaAsync(Guid runId, List<ModelSchema> modelSchemas);

        /// <summary>
        /// Get all runs by experiment id
        /// </summary>
        /// <param name="experimentId"></param>
        /// <returns></returns>
        List<Run> GetRuns(Guid experimentId);

        /// <summary>
        /// Get run by run id
        /// </summary>
        /// <param name="runId"></param>
        /// <returns></returns>
        Run GetRun(Guid runId);

        /// <summary>
        /// Get a run by commit hash
        /// </summary>
        /// <param name="commitHash"></param>
        /// <returns></returns>
        Run GetRun(string commitHash);

        /// <summary>
        /// Sets the training time for a run
        /// </summary>
        /// <param name="runId"></param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        Task SetTrainingTimeAsync(Guid runId, TimeSpan timeSpan);

        /// <summary>
        /// Creates a run artifact for a run
        /// </summary>
        /// <param name="runId"></param>
        /// <param name="name"></param>
        /// <returns>The created RunArtifact</returns>
        Task<RunArtifact> CreateRunArtifact(Guid runId, string name);

        /// <summary>
        /// Gets run artifacts
        /// </summary>
        /// <param name="runId"></param>
        /// <returns></returns>
        List<RunArtifact> GetRunArtifacts(Guid runId);

        /// <summary>
        /// Create a registered model
        /// </summary>
        /// <param name="experimentId"></param>
        /// <param name="runArtifactId"></param>
        /// <param name="registeredBy"></param>
        /// <param name="modelDescription"></param>
        /// <returns></returns>
        Task CreateRegisteredModelAsync(Guid experimentId, Guid runArtifactId, string registeredBy, string modelDescription);

        /// <summary>
        /// Gets all registered models for a given experiement 
        /// </summary>
        /// <param name="experimentId"></param>
        /// <returns></returns>
        List<RegisteredModel> GetRegisteredModels(Guid experimentId);

        /// <summary>
        /// Gets the latest registered model for a given experiment
        /// </summary>
        /// <param name="experimentId"></param>
        /// <returns></returns>
        RegisteredModel GetLatestRegisteredModel(Guid experimentId);
    }
}
