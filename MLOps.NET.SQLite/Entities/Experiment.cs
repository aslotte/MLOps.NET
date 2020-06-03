using MLOps.NET.Entities.Entities;
using System;

namespace MLOps.NET.SQLite.Entities
{
    public sealed class Experiment : IExperiment
    {
        public Experiment(string experimentName)
        {
            Id = Guid.NewGuid();
            ExperimentName = experimentName;
        }

        public Guid Id { get; set; }

        public string ExperimentName { get; set; }
    }
}
