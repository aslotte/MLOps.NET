using Microsoft.EntityFrameworkCore;
using MLOps.NET.Entities.Impl;

namespace MLOps.NET.Azure.Storage
{
    /// <summary>
    /// Configures entity maps for the CosmosDb storage provider
    /// </summary>
    public class CosmosEntityConfigurator
    {
        /// <summary>
        /// Func to configure entity maps
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Experiment>().ToContainer(nameof(Experiment));
            modelBuilder.Entity<Run>().ToContainer(nameof(Run));
            modelBuilder.Entity<HyperParameter>().ToContainer(nameof(HyperParameter));
            modelBuilder.Entity<Metric>().ToContainer(nameof(Metric));
            modelBuilder.Entity<ConfusionMatrixEntity>().ToContainer("ConfusionMatrix");
            modelBuilder.Entity<Data>().ToContainer(nameof(Data));
            modelBuilder.Entity<DataSchema>().ToContainer(nameof(DataSchema));
            modelBuilder.Entity<DataColumn>().ToContainer(nameof(DataColumn));
            modelBuilder.Entity<DataDistribution>().ToContainer(nameof(DataDistribution));
            modelBuilder.Entity<RunArtifact>().ToContainer(nameof(RunArtifact));
            modelBuilder.Entity<RegisteredModel>().ToContainer(nameof(RegisteredModel));
            modelBuilder.Entity<DeploymentTarget>().ToContainer(nameof(DeploymentTarget));
            modelBuilder.Entity<Deployment>().ToContainer(nameof(Deployment));
            modelBuilder.Entity<PackageDependency>().ToContainer(nameof(PackageDependency));
            modelBuilder.Entity<ModelSchema>().ToContainer(nameof(ModelSchema));
        }
    }
}
