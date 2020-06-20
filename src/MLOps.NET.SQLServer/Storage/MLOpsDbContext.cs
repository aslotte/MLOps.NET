using Microsoft.EntityFrameworkCore;
using MLOps.NET.SQLServer.Entities;
using System;
using System.Threading.Tasks;

namespace MLOps.NET.SQLServer.Storage
{
    internal interface IMLOpsDbContext : IDisposable
    {
        DbSet<Experiment> Experiments { get; set; }

        DbSet<Metric> Metrics { get; set; }

        DbSet<HyperParameter> HyperParameters { get; set; }

        DbSet<Run> Runs { get; set; }

        Task SaveChangesAsync();
    }

    internal class MLOpsDbContext : DbContext, IMLOpsDbContext
    {
        private readonly string connectionString;

        public MLOpsDbContext(string connectionString)
        {
            Database.EnsureCreated();
            this.connectionString = connectionString;
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
    }
}
