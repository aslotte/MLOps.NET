using MLOps.NET.Storage;

namespace MLOps.NET.SQLite
{
    public static class MLLifeCycleManagerExtensions
    {
        public static MLLifeCycleManager UseSQLite(this MLLifeCycleManager mLLifeCycleManager)
        {
            mLLifeCycleManager.MetaDataStore = new SQLiteMetaDataStore();

            return mLLifeCycleManager;
        }
    }
}
