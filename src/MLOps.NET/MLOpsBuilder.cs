using MLOps.NET.Storage;
using MLOps.NET.Storage.Database;
using System;

namespace MLOps.NET
{
    /// <summary>
    /// Chained builder for creating an <see cref="IMLOpsContext"/>
    /// </summary>
    public class MLOpsBuilder
    {
        private IExperimentRepository experimentRepository;
        private IRunRepository runRepository;
        private IDataRepository dataRepository;
        private IMetricRepository metricRepository;
        private IConfusionMatrixRepository confusionMatrixRepository;
        private IHyperParameterRepository hyperParameterRepository;

        private IModelRepository modelRepository;

        /// <summary>
        /// Build the <see cref="IMLOpsContext"/> using the provided configuration
        /// </summary>
        /// <returns>Configured <see cref="IMLOpsContext"/></returns>
        public IMLOpsContext Build()
        {
            return new MLOpsContext(modelRepository, experimentRepository, runRepository, dataRepository, metricRepository, confusionMatrixRepository, hyperParameterRepository);
        }

        /// <summary>
        /// Configures the builder to use a repository
        /// </summary>
        /// <returns>The current builder for chaining</returns>
        public MLOpsBuilder UseMetaDataRepositories(IDbContextFactory contextFactory)
        {
            this.experimentRepository = new ExperimentRepository(contextFactory);
            this.runRepository = new RunRepository(contextFactory);
            this.dataRepository = new DataRepository(contextFactory);
            this.metricRepository = new MetricRepository(contextFactory);
            this.confusionMatrixRepository = new ConfusionMatrixRepository(contextFactory);
            this.hyperParameterRepository = new HyperParameterRepository(contextFactory);

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