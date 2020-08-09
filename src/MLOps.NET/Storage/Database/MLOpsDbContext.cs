using Microsoft.EntityFrameworkCore;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage.Interfaces;
using System.Reflection;
using System.Threading.Tasks;

namespace MLOps.NET.Storage.Database
{
    ///<inheritdoc cref="IMLOpsDbContext"/>
    public class MLOpsDbContext : DbContext, IMLOpsDbContext
    {
        ///<inheritdoc cref="IMLOpsDbContext"/>
        public MLOpsDbContext(DbContextOptions options) : base(options)
        {
        }

        ///<inheritdoc cref="IMLOpsDbContext"/>
        protected new virtual void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        ///<inheritdoc cref="IMLOpsDbContext"/>
        public async Task SaveChangesAsync()
        {
            await base.SaveChangesAsync();
        }

        /// <summary>
        /// Ensured that the database is created
        /// </summary>
        /// <returns></returns>
        public virtual void EnsureCreated()
        {
            Database.Migrate();
        }

        ///<inheritdoc cref="IMLOpsDbContext"/>
        public DbSet<Experiment> Experiments { get; set; }

        ///<inheritdoc cref="IMLOpsDbContext"/>
        public DbSet<Metric> Metrics { get; set; }

        ///<inheritdoc cref="IMLOpsDbContext"/>
        public DbSet<HyperParameter> HyperParameters { get; set; }

        ///<inheritdoc cref="IMLOpsDbContext"/>
        public DbSet<Run> Runs { get; set; }

        ///<inheritdoc cref="IMLOpsDbContext"/>
        public DbSet<ConfusionMatrixEntity> ConfusionMatrices { get; set; }

        ///<inheritdoc cref="IMLOpsDbContext"/>
        public DbSet<Data> Data { get; set; }

        ///<inheritdoc cref="IMLOpsDbContext"/>
        public DbSet<RunArtifact> RunArtifacts { get; set; }

        ///<inheritdoc cref="IMLOpsDbContext"/>
        public DbSet<RegisteredModel> RegisteredModels { get; set; }

        ///<inheritdoc cref="IMLOpsDbContext"/>
        public DbSet<DeploymentTarget> DeploymentTargets { get; set; }

        ///<inheritdoc cref="IMLOpsDbContext"/>
        public DbSet<Deployment> Deployments { get; set; }
    }
}
