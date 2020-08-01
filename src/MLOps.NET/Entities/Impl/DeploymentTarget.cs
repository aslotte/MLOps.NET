using System;

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
            Id = Guid.NewGuid();
            Name = name;
        }

        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// CreatedDate
        /// </summary>
        public DateTime CreatedDate { get; set; }
    }
}
