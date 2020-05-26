using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;

namespace MLOps.NET.Entities
{
    internal sealed class Run : TableEntity
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
