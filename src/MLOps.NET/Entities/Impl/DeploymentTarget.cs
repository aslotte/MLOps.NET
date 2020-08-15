using System;
using System.Collections.Generic;

namespace MLOps.NET.Entities.Impl
{
    /// <summary>
    /// Entity describing a deployment target
    /// </summary>
    public sealed class DeploymentTarget
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name"></param>
        public DeploymentTarget(string name)
        {
            DeploymentTargetId = Guid.NewGuid();
            Name = name;
        }

        /// <summary>
        /// Id
        /// </summary>
        public Guid DeploymentTargetId { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// CreatedDate
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Deployments
        /// </summary>
        public List<Deployment> Deployments { get; set; }

        /// <summary>
        /// Flag indicating whether DeploymentTarget is a Production environment or not.
        /// </summary>
        public bool IsProduction { get; set; }
    }
}
