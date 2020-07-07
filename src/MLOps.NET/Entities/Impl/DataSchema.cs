using MLOps.NET.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MLOps.NET.Entities.Impl
{
    ///<inheritdoc cref="IDataSchema"/>
    public sealed class DataSchema : IDataSchema
    {
        ///<inheritdoc cref="IDataSchema"/>
        public DataSchema(Guid dataId)
        {
            Id = Guid.NewGuid();
            DataId = dataId;
        }

        ///<inheritdoc cref="IDataSchema"/>
        public Guid Id { get; set; }

        ///<inheritdoc cref="IDataSchema"/>
        public Guid DataId { get; set; }

        ///<inheritdoc cref="IDataSchema"/>
        public int ColumnCount { get; set; }

        ///<inheritdoc cref="IDataSchema"/>
        [NotMapped]
        public List<IDataColumn> DataColumns { get; set; }
    }
}
