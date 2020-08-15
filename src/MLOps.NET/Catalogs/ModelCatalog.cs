using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MLOps.NET.Catalogs
{
    /// <summary>
    /// Operations related to a model
    /// </summary>
    public sealed class ModelCatalog
    {
        private readonly IModelRepository modelRepository;
        private readonly IRunRepository runRepository;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="modelRepository"></param>
        /// <param name="runRepository"></param>
        public ModelCatalog(IModelRepository modelRepository, IRunRepository runRepository)
        {
            this.modelRepository = modelRepository;
            this.runRepository = runRepository;
        }

        ///<inheritdoc/>
        public async Task UploadAsync(Guid runId, string filePath)
        {
            var artifactName = $"{runId}.zip";
            await runRepository.CreateRunArtifact(runId, artifactName);
            await modelRepository.UploadModelAsync(runId, filePath);
        }

        /// <summary>
        /// Downloads the model data for the provided run ID into the stream
        /// </summary>
        /// <param name="runId">Run ID to download model data for</param>
        /// <param name="stream">Stream to populate with loaded data</param>
        /// <returns>Task containing result of operation</returns>
        public Task DownloadAsync(Guid runId, Stream stream)
        {
            return modelRepository.DownloadModelAsync(runId, stream);
        }

        /// <summary>
        /// Get run artifacts for a run
        /// </summary>
        /// <param name="runId"></param>
        /// <returns></returns>
        public List<RunArtifact> GetRunArtifacts(Guid runId)
        {
            return runRepository.GetRunArtifacts(runId);
        }

        /// <summary>
        /// Registers a model
        /// </summary>
        /// <param name="experimentId"></param>
        /// <param name="runArtifactId"></param>
        /// <param name="registeredBy"></param>
        /// <param name="modelDescription"></param>
        /// <returns>The registered model</returns>
        public async Task<RegisteredModel> RegisterModel(Guid experimentId, Guid runArtifactId, string registeredBy, string modelDescription)
        {
            await runRepository.CreateRegisteredModelAsync(experimentId, runArtifactId, registeredBy, modelDescription);

            return runRepository.GetLatestRegisteredModel(experimentId);
        }

        /// <summary>
        /// Gets all registered models for a given experiement 
        /// </summary>
        /// <param name="experimentId"></param>
        /// <returns></returns>
        public IEnumerable<RegisteredModel> GetRegisteredModels(Guid experimentId)
        {
            return runRepository.GetRegisteredModels(experimentId);
        }

        /// <summary>
        /// Gets the latest registered model for a given experiment
        /// </summary>
        /// <param name="experimentId"></param>
        /// <returns></returns>
        public RegisteredModel GetLatestRegisteredModel(Guid experimentId)
        {
            return runRepository.GetLatestRegisteredModel(experimentId);
        }
    }
}
