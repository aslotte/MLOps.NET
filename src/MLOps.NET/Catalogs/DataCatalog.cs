using Microsoft.ML;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MLOps.NET.Catalogs
{
    /// <summary>
    /// Operations related to tracking the data used to train a model
    /// </summary>
    public sealed class DataCatalog
    {
        private readonly IDataRepository dataRepository;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="dataRepository"></param>
        public DataCatalog(IDataRepository dataRepository)
        {
            this.dataRepository = dataRepository;
        }

        /// <summary>
        /// Logs the schema, column and row count of the data
        /// </summary>
        /// <param name="runId"></param>
        /// <param name="dataView"></param>
        /// <returns></returns>
        public async Task LogDataAsync(Guid runId, IDataView dataView)
        {
            await this.dataRepository.LogDataAsync(runId, dataView);
        }

        /// <summary>
        /// Gets logged data related to a run
        /// </summary>
        /// <param name="runId"></param>
        /// <returns></returns>
        public Data GetData(Guid runId)
        {
            return this.dataRepository.GetData(runId);
        }

        /// <summary>
        /// Logs the data distribution for a given column
        /// </summary>
        /// <typeparam name="T">represents the type of the column to log the distribution for</typeparam>
        /// <param name="runId"></param>
        /// <param name="dataView"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public async Task LogDataDistribution<T>(Guid runId, IDataView dataView, string columnName) where T : struct
        {
            await this.dataRepository.LogDataDistribution<T>(runId, dataView, columnName);
        }

        /// <summary>
        /// Get Data Distribution
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="runId"></param>
        /// <param name="dataView"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public List<DataDistribution> GetDataDistribution<T>(Guid runId, IDataView dataView, string columnName) where T : struct
        {
            return this.dataRepository.GetDataDistribution<T>(runId, dataView, columnName);
        }

    }
}
