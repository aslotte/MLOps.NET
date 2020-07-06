using MLOps.NET.Entities.Interfaces;
using System;

namespace MLOps.NET.SQLite.Entities
{
    internal sealed class DataColumn : IDataColumn
    {
        public DataColumn(Guid dataSchemaId)
        {
            Id = Guid.NewGuid();
            DataSchemaId = dataSchemaId;
        }

        public Guid Id { get; set; }

        public Guid DataSchemaId { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }
    }
}
