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
        /// RegisteredModelId
        /// </summary>
        public Guid RegisteredModelId { get; set; }

        /// <summary>
        /// ExperimentId
        /// </summary>
        public Guid ExperimentId { get; set; }

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
