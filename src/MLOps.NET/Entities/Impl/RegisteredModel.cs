using System;
using System.Collections.Generic;

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
        /// ExperimentId
        /// </summary>
        public Guid ExperimentId { get; set; }

        /// <summary>
        /// RunId
        /// </summary>
        public Guid RunId { get; set; }

        /// <summary>
        /// Date the model was registered
        /// </summary>
        public DateTime RegisteredDate { get; set; }

        /// <summary>
        /// RegisteredBy
        /// </summary>
        public string RegisteredBy { get; set; }

        /// <summary>
        /// Model Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Version
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Deployments
        /// </summary>
        public List<Deployment> Deployments { get; set; }
    }
}
