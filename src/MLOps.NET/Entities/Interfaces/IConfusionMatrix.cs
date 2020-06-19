using System;
using System.Collections.Generic;
using System.Text;

namespace MLOps.NET.Entities.Entities
{
    /// <summary>
    /// Confusion matrix associated with a classifier.
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
        int NumberOfClasses { get; set; }

        /// <summary>
        /// 
        /// </summary>
        IReadOnlyList<double> PerClassPrecision { get; set; }

        /// <summary>
        /// 
        /// </summary>
        IReadOnlyList<double> PerClassRecall { get; set; }

        /// <summary>
        /// 
        /// </summary>
        IReadOnlyList<IReadOnlyList<double>> Counts { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string SerializedDetails { get; set; }
    }
}
