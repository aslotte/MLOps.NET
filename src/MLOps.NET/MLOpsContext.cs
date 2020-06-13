using MLOps.NET.Catalogs;
using MLOps.NET.Storage;
using System;

namespace MLOps.NET
{
    ///<inheritdoc cref="IMLOpsContext"/>
    public class MLOpsContext : IMLOpsContext
    {
        internal MLOpsContext(IMetaDataStore metaDataStore, IModelRepository modelRepository)
        {
            if (metaDataStore == null) throw new ArgumentNullException(nameof(metaDataStore));
            if (modelRepository == null) throw new ArgumentNullException(nameof(modelRepository));

            this.LifeCycle = new LifeCycleCatalog(metaDataStore);
            this.Evaluation = new EvaluationCatalog(metaDataStore);
            this.Model = new ModelCatalog(modelRepository);
            this.Training = new TrainingCatalog(metaDataStore);
        }

        ///<inheritdoc cref="IMLOpsContext"/>
        public LifeCycleCatalog LifeCycle { get; private set; }

        ///<inheritdoc cref="IMLOpsContext"/>
        public EvaluationCatalog Evaluation { get; private set; }

        ///<inheritdoc cref="IMLOpsContext"/>
        public ModelCatalog Model { get; private set; }

        ///<inheritdoc cref="IMLOpsContext"/>
        public TrainingCatalog Training { get; private set; }

    }
}
