using System;

namespace MLOps.NET.Entities.Impl
{
    /// <summary>
    /// Label associated with a registered model
    /// </summary>
    public class ModelLabel
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public ModelLabel()
        {
            ModelLabelId = Guid.NewGuid();
        }

        /// <summary>
        /// Model Label Id
        /// </summary>
        public Guid ModelLabelId { get; set; }

        /// <summary>
        /// Registered Model Id
        /// </summary>
        public Guid RegisteredModelId { get; set; }

        /// <summary>
        /// Label Name
        /// </summary>
        public string LabelName { get; set; }

        /// <summary>
        /// Label Value
        /// </summary>
        public string LabelValue { get; set; }

    }
}
