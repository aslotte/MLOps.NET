using Microsoft.Azure.Cosmos.Table;
using MLOps.NET.Entities.Interfaces;
using System;

namespace MLOps.NET.Azure.Entities
{
    internal sealed class Data : TableEntity, IData
    {
        public Data() { }

        public Data(Guid runId)
        {
            Id = Guid.NewGuid();
            RowKey = Id.ToString();
            PartitionKey = runId.ToString();
            RunId = runId;
        }

        public Guid Id { get; set; }

        public Guid RunId { get; set; }

        public IDataSchema DataSchema { get; set; } 
    }
}
