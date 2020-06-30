using System;
using System.Collections.Generic;

namespace MLOps.NET.Entities.Interfaces
{
    /// <summary>
    /// Entity describing the data schema
    /// </summary>
    public interface IDataSchema
    {
        /// <summary>
        /// Id for DataSchema
        /// </summary>
        Guid Id { get; set; }

        /// <summary>
        /// Reference to Data
        /// </summary>
        Guid DataId { get; set; }

        /// <summary>
        /// The column count of the data
        /// </summary>
        int ColumnCount { get; set; }

        /// <summary>
        /// Collection of related data columns
        /// </summary>
        List<IDataColumn> DataColumns { get; set; }
    }
}
