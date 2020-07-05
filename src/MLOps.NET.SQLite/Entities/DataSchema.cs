using MLOps.NET.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MLOps.NET.SQLite.Entities
{
    internal sealed class DataSchema : IDataSchema
    {
        public DataSchema(Guid dataId)
        {
            Id = Guid.NewGuid();
            DataId = dataId;
        }

        public Guid Id { get; set; }

        public Guid DataId { get; set; }

        public int ColumnCount { get; set; }

        [NotMapped]
        public List<IDataColumn> DataColumns { get; set; }
    }
}
