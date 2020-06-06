using System;

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
    }
}