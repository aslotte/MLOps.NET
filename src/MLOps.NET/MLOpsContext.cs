using MLOps.NET.Catalogs;
using MLOps.NET.Storage;
using MLOps.NET.Utilities;
using System;

namespace MLOps.NET
{
    ///<inheritdoc cref="IMLOpsContext"/>
    public class MLOpsContext : IMLOpsContext
    {
        internal MLOpsContext(IModelRepository modelRepository, 
            IExperimentRepository experimentRepository, 
            IRunRepository runRepository,
            IDataRepository dataRepository,
            IMetricRepository metricRepository,
            IConfusionMatrixRepository confusionMatrixRepository,
            IHyperParameterRepository hyperParameterRepository)
        {
            if (modelRepository == null) throw new ArgumentNullException(nameof(modelRepository));
            if (experimentRepository == null) throw new ArgumentNullException(nameof(experimentRepository));
            if (runRepository == null) throw new ArgumentNullException(nameof(runRepository));
            if (dataRepository == null) throw new ArgumentNullException(nameof(dataRepository));
            if (metricRepository == null) throw new ArgumentNullException(nameof(metricRepository));
            if (confusionMatrixRepository == null) throw new ArgumentNullException(nameof(confusionMatrixRepository));
            if (hyperParameterRepository == null) throw new ArgumentNullException(nameof(hyperParameterRepository));

            this.LifeCycle = new LifeCycleCatalog(experimentRepository, runRepository, new Clock());
            this.Data = new DataCatalog(dataRepository);
            this.Evaluation = new EvaluationCatalog(metricRepository, confusionMatrixRepository);
            this.Model = new ModelCatalog(modelRepository);
            this.Training = new TrainingCatalog(hyperParameterRepository);
        }

        ///<inheritdoc cref="IMLOpsContext"/>
        public LifeCycleCatalog LifeCycle { get; private set; }

        ///<inheritdoc cref="IMLOpsContext"/>
        public DataCatalog Data { get; set; }

        ///<inheritdoc cref="IMLOpsContext"/>
        public EvaluationCatalog Evaluation { get; private set; }

        ///<inheritdoc cref="IMLOpsContext"/>
        public ModelCatalog Model { get; private set; }

        ///<inheritdoc cref="IMLOpsContext"/>
        public TrainingCatalog Training { get; private set; }

    }
}
