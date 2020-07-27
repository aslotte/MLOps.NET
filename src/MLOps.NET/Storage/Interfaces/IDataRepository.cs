using Microsoft.ML;
using MLOps.NET.Entities.Impl;
using System;
using System.Threading.Tasks;

namespace MLOps.NET.Storage
{
    /// <summary>
    /// Repository to access and manage data related to a run
    /// </summary>
    public interface IDataRepository
    {
        /// <summary>
        /// Logs the data
        /// </summary>
        /// <param name="runId"></param>
        /// <param name="dataView"></param>
        /// <returns></returns>
        Task LogDataAsync(Guid runId, IDataView dataView);

        /// <summary>
        /// Gets the data associated with a run
        /// </summary>
        /// <param name="runId"></param>
        /// <returns></returns>
        Data GetData(Guid runId);

      /// <summary>
      /// 
      /// </summary>
      /// <param name="runId"></param>
      /// <param name="dataView"></param>
      /// <param name="columnName"></param>
      /// <returns></returns>
        Task LogDataDistribution(Guid runId, IDataView dataView, string columnName);
    }
}
