using System;
using System.Collections.Generic;
using System.Text;

namespace MLOps.NET.Entities.Entities
{
    /// <summary>
    /// Confusion matrix associated with a classifier.
    /// </summary>
    public interface IConfusionMatrixEntity
    {
        /// <summary>
        /// RunId
        /// </summary>
        Guid RunId { get; set; }

        /// <summary>
        /// SerializedMatrix
        /// </summary>
        string SerializedMatrix { get; set; }
    }
}
