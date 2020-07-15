using Microsoft.EntityFrameworkCore;
using MLOps.NET.Entities.Impl;

namespace MLOps.NET.Storage.EntityConfiguration
{
    /// <summary>
    /// Entity map configurator for a relational storage provider
    /// </summary>
    public class RelationalEntityConfigurator
    {
        /// <summary>
        /// Func to configure entity maps
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Experiment>().ToTable(nameof(Experiment));
            modelBuilder.Entity<Run>().ToTable(nameof(Run));
            modelBuilder.Entity<HyperParameter>().ToTable(nameof(HyperParameter));
            modelBuilder.Entity<Metric>().ToTable(nameof(Metric));
            modelBuilder.Entity<ConfusionMatrixEntity>().ToTable("ConfusionMatrix");
            modelBuilder.Entity<Data>().ToTable(nameof(Data));
            modelBuilder.Entity<DataColumn>().ToTable(nameof(DataColumn));
            modelBuilder.Entity<DataSchema>().ToTable(nameof(DataSchema));
            modelBuilder.Entity<RunArtifact>().ToTable(nameof(RunArtifact));
        }
    }
}
