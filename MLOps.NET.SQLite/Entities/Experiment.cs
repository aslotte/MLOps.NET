using MLOps.NET.Entities.Entities;
using System;
using System.Collections.Generic;

namespace MLOps.NET.SQLite.Entities
{
    internal sealed class Experiment : IExperiment
    {
        public Experiment(string experimentName)
        {
            ExperimentName = experimentName;
        }

        public Guid Id { get; set; }

        public string ExperimentName { get; set; }

        public IList<IRun> Runs { get; set; }
    }
}
