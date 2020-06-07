using Microsoft.Azure.Cosmos.Table;
using MLOps.NET.Entities.Entities;
using System;
using System.Collections.Generic;

namespace MLOps.NET.Azure.Entities
{
    internal sealed class Experiment : TableEntity, IExperiment
    {
        public Experiment(string experimentName)
        {
            Id = Guid.NewGuid();
            PartitionKey = experimentName;
            RowKey = experimentName;
        }

        public Experiment()
        {

        }


        public Guid Id { get; set; }

        public string ExperimentName { get; set; }
    }
}
