using MLOps.NET.Storage;

namespace MLOps.NET.SQLite
{
    public static class MLLifeCycleManagerExtensions
    {
        public static MLLifeCycleManager UseSQLite(this MLLifeCycleManager mLLifeCycleManager)
        {
            mLLifeCycleManager.MetaDataStore = new SQLiteMetaDataStore();
            mLLifeCycleManager.ModelRepository = new LocalFileModelRepository();

            return mLLifeCycleManager;
        }
    }
}
