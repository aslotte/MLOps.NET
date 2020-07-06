using Microsoft.Azure.Cosmos.Table;
using MLOps.NET.Entities.Interfaces;
using System;

namespace MLOps.NET.Azure.Entities
{
    internal sealed class DataColumn : TableEntity, IDataColumn
    {
        public DataColumn() { }

        public DataColumn(Guid dataSchemaId)
        {
            Id = Guid.NewGuid();
            RowKey = Id.ToString();
            PartitionKey = dataSchemaId.ToString();
            DataSchemaId = dataSchemaId;
        }

        public Guid Id { get; set; }

        public Guid DataSchemaId { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }
    }
}
