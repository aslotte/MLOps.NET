using System;

namespace MLOps.NET.Entities.Impl
{
    /// <summary>
    /// Metric associated with a given training run
    /// </summary>
    public sealed class ModelSchema
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="runId"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public ModelSchema(Guid runId, string name, string value)
        {
            ModelSchemaId = Guid.NewGuid();
            Name = name;
            Value = value;
            RunId = runId;
        }

        /// <summary>
        /// Id
        /// </summary>
        public Guid ModelSchemaId { get; set; }

        /// <summary>
        /// Name of metric, e.g. Accurarcy, F1Score or Precision/Recall
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// RunId
        /// </summary>
        public Guid RunId { get; set; }
    }
}
