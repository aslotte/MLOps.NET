using System;

namespace MLOps.NET.Entities.Impl
{
    /// <summary>
    /// Run artifact associated with a run, e.g. a trained model
    /// </summary>
    public class RunArtifact
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public RunArtifact()
        {
            RunArtifactId = Guid.NewGuid();
        }

        /// <summary>
        /// RunArtifactId
        /// </summary>
        public Guid RunArtifactId { get; set; }

        /// <summary>
        /// RunId
        /// </summary>
        public Guid RunId { get; set; }

        /// <summary>
        /// Name, e.g. {runId}.zip
        /// </summary>
        public string Name { get; set; }
    }
}
