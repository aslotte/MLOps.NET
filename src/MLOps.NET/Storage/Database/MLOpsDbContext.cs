﻿using Microsoft.EntityFrameworkCore;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage.Interfaces;
using System;
using System.Threading.Tasks;

namespace MLOps.NET.Storage.Database
{
    ///<inheritdoc cref="IMLOpsDbContext"/>
    public class MLOpsDbContext : DbContext, IMLOpsDbContext
    {
        private readonly Action<ModelBuilder> OnModelCreatingAction;

        ///<inheritdoc cref="IMLOpsDbContext"/>
        public MLOpsDbContext(DbContextOptions options, Action<ModelBuilder> configureEntityMap) : base(options)
        {
            this.OnModelCreatingAction = configureEntityMap;
        }

        /// <summary>
        /// Configures the entity maps
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            this.OnModelCreatingAction(modelBuilder);
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
        public void EnsureCreated()
        {
            Database.EnsureCreated();
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
    }
}
