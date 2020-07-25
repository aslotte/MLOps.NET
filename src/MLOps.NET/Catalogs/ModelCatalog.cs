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
        /// <param name="runArtifactId"></param>
        /// <param name="registeredBy"></param>
        public void RegisterModel(Guid runArtifactId, string registeredBy)
        {
            runRepository.CreateRegisteredModel(runArtifactId, registeredBy);
        }
    }
}
