using MLOps.NET.Entities.Impl;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MLOps.NET.Storage
{
    /// <summary>
    /// Repository to access and manage experiments
    /// </summary>
    public interface IExperimentRepository
    {
        /// <summary>
        /// Create an experiment with a provided name
        /// </summary>
        /// <param name="experimentName"></param>
        /// <returns></returns>
        Task<Guid> CreateExperimentAsync(string experimentName);

        /// <summary>
        /// Get all experiments 
        /// </summary>
        /// <returns></returns>
        IEnumerable<Experiment> GetExperiments();

        /// <summary>
        /// Get specific experiment by name
        /// </summary>
        /// <param name="experimentName"></param>
        /// <returns></returns>
        Experiment GetExperiment(string experimentName);

        /// <summary>
        /// Gets an experiment by id
        /// </summary>
        /// <param name="experimentId"></param>
        /// <returns></returns>
        Experiment GetExperiment(Guid experimentId);
    }
}
