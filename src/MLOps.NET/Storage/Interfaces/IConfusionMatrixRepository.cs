using MLOps.NET.Entities;
using System;
using System.Threading.Tasks;

namespace MLOps.NET.Storage
{
    /// <summary>
    /// Repository to access and manage confusion matrices
    /// </summary>
    public interface IConfusionMatrixRepository
    {
        /// <summary>
        /// Saves the confusion matrix as a json serialized string
        /// </summary>
        /// <param name="runId"></param>
        /// <param name="confusionMatrix"></param>
        /// <returns></returns>
        Task LogConfusionMatrixAsync(Guid runId, ConfusionMatrix confusionMatrix);

        /// <summary>
        /// Gets the confusion matrix for a run.
        /// </summary>
        /// <param name="runId"></param>
        /// <returns></returns>
        ConfusionMatrix GetConfusionMatrix(Guid runId);
    }
}
