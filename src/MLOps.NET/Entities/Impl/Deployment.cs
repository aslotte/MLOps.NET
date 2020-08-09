using System;

namespace MLOps.NET.Entities.Impl
{
    /// <summary>
    /// Entity describing a deployment
    /// </summary>
    public sealed class Deployment
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public Deployment()
        {
            DeploymentId = Guid.NewGuid();
        }

        /// <summary>
        /// DeploymentId
        /// </summary>
        public Guid DeploymentId { get; set; }

        /// <summary>
        /// DeploymentTargetId
        /// </summary>
        public Guid DeploymentTargetId { get; set; }

        /// <summary>
        /// DeploymentTarget
        /// </summary>
        public DeploymentTarget DeploymentTarget { get; set; }

        /// <summary>
        /// RegisteredModelId
        /// </summary>
        public Guid RegisteredModelId { get; set; }

        /// <summary>
        /// RegisterdModel
        /// </summary>
        public RegisteredModel RegisteredModel { get; set; }

        /// <summary>
        /// ExperimentId
        /// </summary>
        public Guid ExperimentId { get; set; }

        /// <summary>
        /// Experiment
        /// </summary>
        public Experiment Experiment { get; set; }

        /// <summary>
        /// DeploymentDate
        /// </summary>
        public DateTime DeploymentDate { get; set; }

        /// <summary>
        /// DeployedBy
        /// </summary>
        public string DeployedBy { get; set; }
    }
}
