using Microsoft.EntityFrameworkCore;
using MLOps.NET.SQLServer.Entities;

namespace MLOps.NET.SQLServer.Storage
{
    internal class MLOpsDbContext : DbContext
    {
        private readonly string connectionString;

        public MLOpsDbContext(string connectionString)
        {
            Database.EnsureCreated();
            this.connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlServer(this.connectionString);

        public DbSet<Experiment> Experiments { get; set; }
        public DbSet<Metric> Metrics { get; set; }

        public DbSet<HyperParameter> HyperParameters { get; set; }

        public DbSet<Run> Runs { get; set; }
    }
}
