using Microsoft.Azure.Cosmos.Table;
using MLOps.NET.Entities.Interfaces;
using System;
using System.Collections.Generic;

namespace MLOps.NET.Azure.Entities
{
    internal sealed class DataSchema : TableEntity, IDataSchema
    {
        public DataSchema() { }

        public DataSchema(Guid dataId)
        {
            Id = Guid.NewGuid();
            RowKey = Id.ToString();
            PartitionKey = dataId.ToString();
            DataId = dataId;
        }

        public Guid Id { get; set; }

        public Guid DataId { get; set; }

        public int ColumnCount { get; set; }

        public List<IDataColumn> DataColumns { get; set; }
    }
}
