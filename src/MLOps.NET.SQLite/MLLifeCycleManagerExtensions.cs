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
        /// <param name="destinationFolder">Destination folder (optional with default value of C:\MLops)</param>
        /// <returns></returns>
        public static MLLifeCycleManager UseSQLite(this MLLifeCycleManager mLLifeCycleManager, string destinationFolder)
        {
            mLLifeCycleManager.MetaDataStore = new SQLiteMetaDataStore();
            mLLifeCycleManager.ModelRepository = new LocalFileModelRepository(destinationFolder);

            return mLLifeCycleManager;
        }
    }
}
