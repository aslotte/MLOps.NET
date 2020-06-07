using MLOps.NET.Storage;

namespace MLOps.NET.Azure
{
    public static class MLLifeCycleManagerExtensions
    {
        public static ModelContext UseAzureStorage(this ModelContext mLLifeCycleManager, string connectionString)
        {
            mLLifeCycleManager.MetaDataStore = new StorageAccountMetaDataStore(connectionString);
            mLLifeCycleManager.ModelRepository = new StorageAccountModelRepository(connectionString);

            return mLLifeCycleManager;
        }
    }
}
