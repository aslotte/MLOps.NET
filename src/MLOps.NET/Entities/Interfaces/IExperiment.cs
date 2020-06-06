using System;

namespace MLOps.NET.Entities.Entities
{
    /// <summary>
    /// Entity holding information about a unique experiment
    /// </summary>
    public interface IExperiment
    {
        /// <summary>
        /// Id of the experiment
        /// </summary>
        Guid Id { get; set; }
        /// <summary>
        /// Unique name of the experiment
        /// </summary>
        string ExperimentName { get; set; }
    }
}