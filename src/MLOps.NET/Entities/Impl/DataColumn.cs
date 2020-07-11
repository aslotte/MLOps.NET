using System;

namespace MLOps.NET.Entities.Impl
{
    /// <summary>
    /// Entity to hold information about a data column in a dataset
    /// </summary>
    public sealed class DataColumn
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="dataSchemaId"></param>
        public DataColumn(Guid dataSchemaId)
        {
            Id = Guid.NewGuid();
            DataSchemaId = dataSchemaId;
        }

        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// DataSchemaId
        /// </summary>
        public Guid DataSchemaId { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Type
        /// </summary>
        public string Type { get; set; }
    }
}
