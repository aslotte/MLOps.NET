using System;

namespace MLOps.NET.Entities.Impl
{
    /// <summary>
    /// Registered model to be deployed
    /// </summary>
    public class RegisteredModel
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public RegisteredModel()
        {
            RegisteredModelId = Guid.NewGuid();
        }

        /// <summary>
        /// RegisteredModelId
        /// </summary>
        public Guid RegisteredModelId { get; set; }

        /// <summary>
        /// RunArtifactId
        /// </summary>
        public Guid RunArtifactId { get; set; }

        /// <summary>
        /// RunArtifact
        /// </summary>
        public RunArtifact RunArtifact { get; set; }

        /// <summary>
        /// ExperimentId
        /// </summary>
        public Guid ExperimentId { get; set; }

        /// <summary>
        /// Experiment
        /// </summary>
        public Experiment Experiment { get; set; }

        /// <summary>
        /// RunId
        /// </summary>
        public Guid RunId { get; set; }

        /// <summary>
        /// Run
        /// </summary>
        public Run Run { get; set; }

        /// <summary>
        /// Date the model was registered
        /// </summary>
        public DateTime RegisteredDate { get; set; }

        /// <summary>
        /// RegisteredBy
        /// </summary>
        public string RegisteredBy { get; set; }

        /// <summary>
        /// Version
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Model Label Id
        /// </summary>
        public Guid ModelLabelId { get; set; }
    }
}
