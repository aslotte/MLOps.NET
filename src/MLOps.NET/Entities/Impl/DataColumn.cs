using MLOps.NET.Entities.Interfaces;
using System;

namespace MLOps.NET.Entities.Impl
{
    ///<inheritdoc cref="IDataColumn"/>
    public sealed class DataColumn : IDataColumn
    {
        ///<inheritdoc cref="IDataColumn"/>
        public DataColumn(Guid dataSchemaId)
        {
            Id = Guid.NewGuid();
            DataSchemaId = dataSchemaId;
        }

        ///<inheritdoc cref="IDataColumn"/>
        public Guid Id { get; set; }

        ///<inheritdoc cref="IDataColumn"/>
        public Guid DataSchemaId { get; set; }

        ///<inheritdoc cref="IDataColumn"/>
        public string Name { get; set; }

        ///<inheritdoc cref="IDataColumn"/>
        public string Type { get; set; }
    }
}
