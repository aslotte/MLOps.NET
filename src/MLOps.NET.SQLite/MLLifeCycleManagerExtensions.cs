using MLOps.NET.Storage;

namespace MLOps.NET.SQLite
{
    public static class MLLifeCycleManagerExtensions
    {
        public static ModelContext UseSQLite(this ModelContext mLLifeCycleManager)
        {
            mLLifeCycleManager.MetaDataStore = new SQLiteMetaDataStore();

            return mLLifeCycleManager;
        }
    }
}
