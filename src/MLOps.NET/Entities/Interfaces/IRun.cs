using System;
using System.Collections.Generic;
using System.Dynamic;

namespace MLOps.NET.Entities.Entities
{
    /// <summary>
    /// Unique run for a given experiment
    /// </summary>
    public interface IRun
    {
        /// <summary>
        /// Unique RunId
        /// </summary>
        Guid Id { get; set; }
        /// <summary>
        /// Date of run
        /// </summary>
        DateTime RunDate { get; set; }
        /// <summary>
        /// Experiment Id
        /// </summary>
        Guid ExperimentId { get; set; }

        /// <summary>
        /// Collection of metrics
        /// </summary>
        List<IMetric> Metrics { get; set; }

        /// <summary>
        /// Training time
        /// </summary>
        TimeSpan TrainingTime { get; set; }
    }
}