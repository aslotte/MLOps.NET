using System;
using System.Collections.Generic;

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

        /// <summary>
        /// Date of creation of the experiment
        /// </summary>
        DateTime CreatedAt { get; }

        /// <summary>
        /// Collection of related runs
        /// </summary>
        List<IRun> Runs  { get; set; }
    }
}