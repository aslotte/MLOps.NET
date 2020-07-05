using Microsoft.ML;
using MLOps.NET.Entities.Interfaces;
using MLOps.NET.Storage;
using System;
using System.Threading.Tasks;

namespace MLOps.NET.Catalogs
{
    /// <summary>
    /// Operations related to tracking the data used to train a model
    /// </summary>
    public sealed class DataCatalog
    {
        private readonly IMetaDataStore metaDataStore;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="metaDataStore"></param>
        public DataCatalog(IMetaDataStore metaDataStore)
        {
            this.metaDataStore = metaDataStore;
        }

        /// <summary>
        /// Logs the schema, column and row count of the data
        /// </summary>
        /// <param name="runId"></param>
        /// <param name="dataView"></param>
        /// <returns></returns>
        public async Task LogDataAsync(Guid runId, IDataView dataView)
        {
            await this.metaDataStore.LogDataAsync(runId, dataView);
        }

        /// <summary>
        /// Gets logged data related to a run
        /// </summary>
        /// <param name="runId"></param>
        /// <returns></returns>
        public IData GetData(Guid runId)
        {
            return this.metaDataStore.GetData(runId);
        }
    }
}
