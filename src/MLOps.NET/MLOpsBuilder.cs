using System;
using MLOps.NET.Storage;

namespace MLOps.NET
{
    /// <summary>
    /// Chained builder for creating an <see cref="IMLLifeCycleManager"/>
    /// </summary>
    public class MLOpsBuilder
    {
        private IMetaDataStore metaDataStore;
        private IModelRepository modelRepository;

        /// <summary>
        /// Build the <see cref="IMLLifeCycleManager"/> using the provided configuration
        /// </summary>
        /// <returns>Configured <see cref="IMLLifeCycleManager"/></returns>
        public IMLLifeCycleManager Build()
        {
            return new MLLifeCycleManager(metaDataStore, modelRepository);
        }

        /// <summary>
        /// Configures the builder to use the provided <see cref="IMetaDataStore"/>
        /// </summary>
        /// <param name="metaDataStore"><see cref="IMetaDataStore"/> to use in the MLOps pipeline</param>
        /// <returns>The current builder for chaining</returns>
        public MLOpsBuilder UseMetaDataStore(IMetaDataStore metaDataStore)
        {
            if (this.metaDataStore != null) throw new InvalidOperationException("MetaDataStore is already configured");

            this.metaDataStore = metaDataStore ?? throw new ArgumentNullException(nameof(metaDataStore));
            return this;
        }

        /// <summary>
        /// Configures the builder to use the provided <see cref="IModelRepository"/>
        /// </summary>
        /// <param name="modelRepository"><see cref="IModelRepository"/> to use in the MLOps pipeline</param>
        /// <returns>The current builder for chaining</returns>
        public MLOpsBuilder UseModelRepository(IModelRepository modelRepository)
        {
            if (this.modelRepository != null) throw new InvalidOperationException("ModelRepository is already configured");

            this.modelRepository = modelRepository ?? throw new ArgumentNullException(nameof(modelRepository));
            return this;
        }
    }
}