using MLOps.NET.Entities.Entities;
using System;

namespace MLOps.NET.SQLite.Entities
{
    internal sealed class Experiment : IExperiment
    {
        public Experiment(string experimentName)
        {
            Id = Guid.NewGuid();
            ExperimentName = experimentName;
            CreatedAt = DateTime.Now;
        }

        public Guid Id { get; set; }

        public string ExperimentName { get; set; }

        public DateTime CreatedAt { get; }
    }
}
