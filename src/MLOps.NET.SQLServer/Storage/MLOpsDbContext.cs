using Microsoft.EntityFrameworkCore;
using MLOps.NET.SQLServer.Entities;
using MLOps.NET.SQLServer.Storage.Interfaces;
using System.Threading.Tasks;

namespace MLOps.NET.SQLServer.Storage
{
    internal class MLOpsDbContext : DbContext, IMLOpsDbContext
    {
        private readonly string connectionString;

        public MLOpsDbContext(string connectionString)
        {
            this.connectionString = connectionString;
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlServer(this.connectionString);

        public async Task SaveChangesAsync()
        {
            await base.SaveChangesAsync();
        }

        public DbSet<Experiment> Experiments { get; set; }

        public DbSet<Metric> Metrics { get; set; }

        public DbSet<HyperParameter> HyperParameters { get; set; }

        public DbSet<Run> Runs { get; set; }

        public DbSet<ConfusionMatrixEntity> ConfusionMatrices { get; set; }
    }
}
