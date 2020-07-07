using Microsoft.EntityFrameworkCore;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage.Interfaces;
using System.Threading.Tasks;

namespace MLOps.NET.Storage
{
    ///<inheritdoc cref="IMLOpsDbContext"/>
    public class MLOpsDbContext : DbContext, IMLOpsDbContext
    {
        ///<inheritdoc cref="IMLOpsDbContext"/>
        public MLOpsDbContext(DbContextOptions options) : base (options)
        {
            Database.EnsureCreated();
        }

        ///<inheritdoc cref="IMLOpsDbContext"/>
        public async Task SaveChangesAsync()
        {
            await base.SaveChangesAsync();
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
        public DbSet<DataSchema> DataSchemas { get; set; }

        ///<inheritdoc cref="IMLOpsDbContext"/>
        public DbSet<DataColumn> DataColumns { get; set; }
    }
}
