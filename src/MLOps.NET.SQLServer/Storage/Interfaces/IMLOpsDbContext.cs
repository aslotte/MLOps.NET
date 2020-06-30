using Microsoft.EntityFrameworkCore;
using MLOps.NET.SQLServer.Entities;
using System;
using System.Threading.Tasks;

namespace MLOps.NET.SQLServer.Storage.Interfaces
{
    internal interface IMLOpsDbContext : IDisposable
    {
        DbSet<Experiment> Experiments { get; set; }

        DbSet<Metric> Metrics { get; set; }

        DbSet<HyperParameter> HyperParameters { get; set; }

        DbSet<Run> Runs { get; set; }

        DbSet<ConfusionMatrixEntity> ConfusionMatrices { get; set; }

        DbSet<Data> Data { get; set; }

        DbSet<DataSchema> DataSchemas { get; set; }

        DbSet<DataColumn> DataColumns { get; set; }

        Task SaveChangesAsync();
    }
}
