using System;

namespace MLOps.NET.Entities.Impl
{
    /// <summary>
    /// Entity holding information about a model's input or output
    /// </summary>
    public sealed class ModelSchema
    {
        /// <summary>
        /// ModelSchemaId
        /// </summary>
        public Guid ModelSchemaId { get; set; }

        /// <summary>
        /// RunId
        /// </summary>
        public Guid RunId { get; set; }

        /// <summary>
        /// Name of model schema (ModelInput or ModelOutput)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Schema definition
        /// </summary>
        public string Definition { get; set; }
    }
}
