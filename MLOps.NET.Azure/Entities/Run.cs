using Microsoft.Azure.Cosmos.Table;
using MLOps.NET.Entities.Entities;
using System;

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
            RunDate = DateTime.Now;
        }

        public Guid Id { get; set; }

        public DateTime RunDate { get; set; }
    }
}
