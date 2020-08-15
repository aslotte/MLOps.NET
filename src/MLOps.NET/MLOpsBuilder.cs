using MLOps.NET.Storage;
using MLOps.NET.Storage.Database;
using MLOps.NET.Storage.EntityResolvers;
using MLOps.NET.Storage.Repositories;
using MLOps.NET.Utilities;
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
        private DeploymentRepository deploymentRepository;
        private IModelRepository modelRepository;

        /// <summary>
        /// Build the <see cref="IMLOpsContext"/> using the provided configuration
        /// </summary>
        /// <returns>Configured <see cref="IMLOpsContext"/></returns>
        public IMLOpsContext Build()
        {
            return new MLOpsContext(modelRepository, experimentRepository, runRepository, dataRepository, metricRepository, confusionMatrixRepository, hyperParameterRepository, deploymentRepository);
        }

        /// <summary>
        /// Configures the builder to use a repository
        /// </summary>
        /// <returns>The current builder for chaining</returns>
        public MLOpsBuilder UseMetaDataRepositories(IDbContextFactory contextFactory)
        {
            var runResolver = new RunResolver();
            var experimentResolver = new ExperimentResolver(runResolver);

            this.experimentRepository = new ExperimentRepository(contextFactory, experimentResolver);
            this.runRepository = new RunRepository(contextFactory, new Clock(), runResolver, new RegisteredModelResolver());
            this.dataRepository = new DataRepository(contextFactory);
            this.metricRepository = new MetricRepository(contextFactory);
            this.confusionMatrixRepository = new ConfusionMatrixRepository(contextFactory);
            this.hyperParameterRepository = new HyperParameterRepository(contextFactory);
            this.deploymentRepository = new DeploymentRepository(contextFactory, new Clock(), new DeploymentTargetResolver());

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