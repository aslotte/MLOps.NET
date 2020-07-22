using MLOps.NET.Entities.Impl;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MLOps.NET.Storage
{
    /// <summary>
    /// Repository to access and manage labels
    /// </summary>
    public interface IModelLabelRepository
    {
        /// <summary>
        /// Logs a given label for a run
        /// </summary>
        /// <param name="runArtifactId"></param>
        /// <param name="labelName"></param>
        /// <param name="labelValue"></param>
        /// <returns></returns>
        Task LogModelLabelAsync(Guid runArtifactId, string labelName, string labelValue);

        /// <summary>
        /// Get all labels by run artifact id
        /// </summary>
        /// <param name="runArtifactId"></param>
        /// <returns></returns>
        List<ModelLabel> GetModelLabels(Guid runArtifactId);
    }
}
