using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage.Interfaces;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace MLOps.NET.Storage.Database
{
    ///<inheritdoc cref="IMLOpsDbContext"/>
    public class MLOpsDbContext : DbContext, IMLOpsDbContext
    {
        private readonly Action<ModelBuilder> onModelCreating;

        ///<inheritdoc cref="IMLOpsDbContext"/>
        public MLOpsDbContext(DbContextOptions options, Action<ModelBuilder> onModelCreating) : base(options)
        {
            this.onModelCreating = onModelCreating;
        }

        ///<inheritdoc cref="IMLOpsDbContext"/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            this.onModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(MLOpsDbContext)));

            base.OnModelCreating(modelBuilder);
        }

        ///<inheritdoc cref="IMLOpsDbContext"/>
        public async Task SaveChangesAsync()
        {
            await base.SaveChangesAsync();
        }

        ///<inheritdoc cref="IMLOpsDbContext"/>
        public virtual void EnsureCreated()
        {
            Database.Migrate();
        }

        ///<inheritdoc cref="IMLOpsDbContext"/>
        public override EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class
        {
            return base.Entry(entity);
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
