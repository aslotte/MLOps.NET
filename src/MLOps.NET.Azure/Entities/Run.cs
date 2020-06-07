using Microsoft.Azure.Cosmos.Table;
using MLOps.NET.Entities.Entities;
using System;
using System.Collections.Generic;

namespace MLOps.NET.Azure.Entities
{
    internal sealed class Run : TableEntity, IRun
    {
        public Run() { }

        public Run(Guid experimentId)
        {
            Id = Guid.NewGuid();

            PartitionKey = experimentId.ToString();
            RowKey = Id.ToString();
            RunDate = DateTime.UtcNow;
            ExperimentId = experimentId;
        }

        public Guid Id { get; set; }

        public DateTime RunDate { get; set; }

        public Guid ExperimentId { get; set; }

        public List<IMetric> Metrics { get; set; }
    }
}