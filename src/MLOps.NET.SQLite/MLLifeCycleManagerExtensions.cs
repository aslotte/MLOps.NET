using MLOps.NET.Storage;

namespace MLOps.NET.SQLite
{
    /// <summary>
    /// Extension methods to allow the usage of SQLite
    /// </summary>
    public static class MLLifeCycleManagerExtensions
    {
        /// <summary>
        /// Enables the usage of SQLite and local storage
        /// </summary>
        /// <param name="mLLifeCycleManager"></param>
        /// <returns></returns>
        public static MLLifeCycleManager UseSQLite(this MLLifeCycleManager mLLifeCycleManager)
        {
            mLLifeCycleManager.MetaDataStore = new SQLiteMetaDataStore();

            return mLLifeCycleManager;
        }
    }
}
