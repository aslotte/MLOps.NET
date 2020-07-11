using System;
using System.Collections.Generic;

namespace MLOps.NET.Entities.Impl
{
    /// <summary>
    /// Unique run for a given experiment
    /// </summary>
    public sealed class Run
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="experimentId"></param>
        public Run(Guid experimentId)
        {
            Id = Guid.NewGuid();
            RunDate = DateTime.UtcNow;
            ExperimentId = experimentId;
        }

        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// RunDate
        /// </summary>
        public DateTime RunDate { get; set; }

        /// <summary>
        /// ExperimentId
        /// </summary>
        public Guid ExperimentId { get; set; }

        /// <summary>
        /// Metrics
        /// </summary>
        public List<Metric> Metrics { get; set; }

        /// <summary>
        /// TrainingTime
        /// </summary>
        public TimeSpan TrainingTime { get; set; }

        /// <summary>
        /// GitCommitHash
        /// </summary>
        public string GitCommitHash { get; set; }
    }
}
