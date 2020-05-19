using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;

namespace MLOps.NET.Entities
{
    internal sealed class Experiment : TableEntity
    {
        public Experiment(string experimentName)
        {
            Id = Guid.NewGuid();
            PartitionKey = experimentName;
            RowKey = Id.ToString();
        }

        public Guid Id { get; set; }

        public string ExperimentName { get; set; }

        public List<Run> Runs { get; set; }
    }
}
