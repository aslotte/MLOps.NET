using System;
using System.Collections.Generic;

namespace MLOps.NET.Entities.Impl
{
    /// <summary>
    /// Entity holding information about a unique experiment
    /// </summary>
    public sealed class Experiment
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="experimentName"></param>
        public Experiment(string experimentName)
        {
            Id = Guid.NewGuid();
            ExperimentName = experimentName;
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ExperimentName
        /// </summary>
        public string ExperimentName { get; set; }

        /// <summary>
        /// CreatedAt
        /// </summary>
        public DateTime CreatedAt { get; }

        /// <summary>
        /// Runs
        /// </summary>
        public List<Run> Runs { get; set; }
    }
}
