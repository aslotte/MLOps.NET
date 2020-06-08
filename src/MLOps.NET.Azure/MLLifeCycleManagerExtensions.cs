using MLOps.NET.Storage;

namespace MLOps.NET.Azure
{
    /// <summary>
    /// Extension methods to allow the usage of Azure storage
    /// </summary>
    public static class MLLifeCycleManagerExtensions
    {
        /// <summary>
        /// Enables the usage of Azure Blobstorage and TableStorage as a storage provider
        /// </summary>
        /// <param name="mLLifeCycleManager"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static MLLifeCycleManager UseAzureStorage(this MLLifeCycleManager mLLifeCycleManager, string connectionString)
        {
            mLLifeCycleManager.MetaDataStore = new StorageAccountMetaDataStore(connectionString);
            mLLifeCycleManager.ModelRepository = new StorageAccountModelRepository(connectionString);

            return mLLifeCycleManager;
        }
    }
}
