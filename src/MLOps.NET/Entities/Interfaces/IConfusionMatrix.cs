using System;

namespace MLOps.NET.Entities.Entities
{
    /// <summary>
    /// Confusion matrix associated with a given training run
    /// </summary>
    public interface IConfusionMatrix
    {
        /// <summary>
        /// 
        /// </summary>
        Guid RunId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string Value { get; set; }
    }
}