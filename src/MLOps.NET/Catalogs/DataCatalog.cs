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
        public List<Data> GetData(Guid runId)
        {
            return this.dataRepository.GetData(runId);
        }
    }
}
