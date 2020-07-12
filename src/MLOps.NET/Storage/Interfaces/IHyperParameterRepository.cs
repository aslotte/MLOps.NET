using System;
using System.Threading.Tasks;

namespace MLOps.NET.Storage
{
    /// <summary>
    /// Repository to access and manage hyperparameters
    /// </summary>
    public interface IHyperParameterRepository
    {
        /// <summary>
        /// Logs a given hyperparameter for a run
        /// </summary>
        /// <param name="runId"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task LogHyperParameterAsync(Guid runId, string name, string value);
    }
}
