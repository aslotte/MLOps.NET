using System;

namespace MLOps.NET.Entities.Interfaces
{
    /// <summary>
    /// Entity to hold information about a data column in a dataset
    /// </summary>
    public interface IDataColumn
    {
        /// <summary>
        /// Id of the column
        /// </summary>
        Guid Id { get; set; }

        /// <summary>
        /// Reference to the DataSchema
        /// </summary>
        Guid DataSchemaId { get; set; }

        /// <summary>
        /// Name of the column
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Type of the column
        /// </summary>
        string Type { get; set; }
    }
}
