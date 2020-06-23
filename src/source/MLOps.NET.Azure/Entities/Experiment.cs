using Microsoft.Azure.Cosmos.Table;
using MLOps.NET.Entities.Entities;
using System;
using System.Collections.Generic;

namespace MLOps.NET.Azure.Entities
{
    internal sealed class Experiment : TableEntity, IExperiment
    {

        public Experiment() { }

        public Experiment(string experimentName)
        {
            Id = Guid.NewGuid();
            PartitionKey = experimentName;
            RowKey = experimentName;
            CreatedAt = DateTime.UtcNow;
        }

        public Guid Id { get; set; }

        public string ExperimentName { get; set; }

        public DateTime CreatedAt { get; }

        public List<IRun> Runs { get; set; }
    }
}
