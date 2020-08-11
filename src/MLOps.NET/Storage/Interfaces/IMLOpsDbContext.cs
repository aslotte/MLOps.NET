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
        /// Run artifact associated with a run
        /// </summary>
        DbSet<RunArtifact> RunArtifacts { get; set; }

        /// <summary>
        /// RegisteredModels
        /// </summary>
        DbSet<RegisteredModel> RegisteredModels { get; set; }

        /// <summary>
        /// DeploymentTargets
        /// </summary>
        DbSet<DeploymentTarget> DeploymentTargets { get; set; }

        /// <summary>
        /// Deployments
        /// </summary>
        DbSet<Deployment> Deployments { get; set; }

        /// <summary>
        /// 
        /// </summary>
        DbSet<DataDistribution> DataDistributions { get; set; }

        /// <summary>
        /// Save changes
        /// </summary>
        /// <returns></returns>
        Task SaveChangesAsync();

        /// <summary>
        /// Creates the database if it does not exist
        /// Applies any pending migrations for a relational database
        /// </summary>
        /// <returns></returns>
        void EnsureCreated();
    }
}
