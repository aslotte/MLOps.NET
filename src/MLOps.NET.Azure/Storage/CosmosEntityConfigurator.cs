using Microsoft.EntityFrameworkCore;
using MLOps.NET.Entities.Impl;
using System.Security.Cryptography.X509Certificates;

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
            modelBuilder.Entity<RunArtifact>().ToContainer(nameof(RunArtifact));

            modelBuilder.Entity<Data>().OwnsOne(x => x.DataSchema, dataSchema =>
            {
                dataSchema.OwnsMany(x => x.DataColumns);
            });
        }
    }
}
