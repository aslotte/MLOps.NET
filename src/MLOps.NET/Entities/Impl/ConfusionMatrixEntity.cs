using System;

namespace MLOps.NET.Entities.Impl
{
    /// <summary>
    /// Confusion matrix associated with a classifier.
    /// </summary>
    public sealed class ConfusionMatrixEntity
    {

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="runId"></param>
        public ConfusionMatrixEntity(Guid runId)
        {
            RunId = runId;
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// RunId
        /// </summary>
        public Guid RunId { get; set; }

        /// <summary>
        /// SerializedMatrix
        /// </summary>
        public string SerializedMatrix { get; set; }
    }
}
