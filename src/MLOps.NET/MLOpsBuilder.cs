using System;
using MLOps.NET.Storage;

namespace MLOps.NET
{
    public class MLOpsBuilder
    {
        private IMetaDataStore metaDataStore;
        private IModelRepository modelRepository;

        public IMLLifeCycleManager Build()
        {
            return new MLLifeCycleManager(metaDataStore, modelRepository);
        }

        public MLOpsBuilder UseMetaDataStore(IMetaDataStore metaDataStore)
        {
            if (this.metaDataStore != null) throw new InvalidOperationException("MetaDataStore is already configured");

            this.metaDataStore = metaDataStore ?? throw new ArgumentNullException(nameof(metaDataStore));
            return this;
        }

        public MLOpsBuilder UseModelRepository(IModelRepository modelRepository)
        {
            if (this.modelRepository != null) throw new InvalidOperationException("ModelRepository is already configured");

            this.modelRepository = modelRepository ?? throw new ArgumentNullException(nameof(modelRepository));
            return this;
        }
    }
}