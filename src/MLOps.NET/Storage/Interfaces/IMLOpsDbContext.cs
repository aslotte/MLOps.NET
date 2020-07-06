using Microsoft.EntityFrameworkCore;
using MLOps.NET.Entities.Impl;
using System;
using System.Threading.Tasks;

namespace MLOps.NET.Storage.Interfaces
{
    /// <summary>
    /// Operations related to accessing entities
    /// </summary>
    public interface IMLOpsDbContext : IDisposable
    {
        /// <summary>
        /// Experiments
        /// </summary>
        DbSet<Experiment> Experiments { get; set; }

        /// <summary>
        /// Metrics
        /// </summary>
        DbSet<Metric> Metrics { get; set; }

        /// <summary>
        /// Hyperparameters
        /// </summary>
        DbSet<HyperParameter> HyperParameters { get; set; }

        /// <summary>
        /// Runs
        /// </summary>
        DbSet<Run> Runs { get; set; }

        /// <summary>
        /// ConfusionMatrices
        /// </summary>
        DbSet<ConfusionMatrixEntity> ConfusionMatrices { get; set; }

        /// <summary>
        /// Data
        /// </summary>
        DbSet<Data> Data { get; set; }

        /// <summary>
        /// DataSchemas
        /// </summary>
        DbSet<DataSchema> DataSchemas { get; set; }

        /// <summary>
        /// DataColumns
        /// </summary>
        DbSet<DataColumn> DataColumns { get; set; }

        /// <summary>
        /// Save changes
        /// </summary>
        /// <returns></returns>
        Task SaveChangesAsync();
    }
}
