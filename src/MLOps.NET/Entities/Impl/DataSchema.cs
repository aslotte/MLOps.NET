using System;
using System.Collections.Generic;

namespace MLOps.NET.Entities.Impl
{
    /// <summary>
    /// Entity describing the data schema
    /// </summary>
    public sealed class DataSchema
    {
        /// <summary>
        /// Ctr
        /// </summary>
        public DataSchema()
        {
            DataSchemaId = Guid.NewGuid();
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="dataId"></param>
        public DataSchema(Guid dataId)
        {
            DataSchemaId = Guid.NewGuid();
            DataId = dataId;
        }

        /// <summary>
        /// RunId
        /// </summary>
        public Guid DataSchemaId { get; set; }

        /// <summary>
        /// DataId
        /// </summary>
        public Guid DataId { get; set; }

        /// <summary>
        /// ColumnCount
        /// </summary>
        public int ColumnCount { get; set; }

        /// <summary>
        /// DataColumns
        /// </summary>
        public List<DataColumn> DataColumns { get; set; } = new List<DataColumn>();
    }
}
