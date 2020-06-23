using System;
using System.Collections.Generic;
using System.Text;

namespace MLOps.NET.Entities
{
    /// <summary>
    /// Mirror image class for Ml.net Confusionmatrix
    /// </summary>
    public class ConfusionMatrix
    {
        /// <summary>
        /// number of clssses in a classifier
        /// </summary>
        public int NumberOfClasses { get; set; }

        /// <summary>
        /// per class precision
        /// </summary>
        public IReadOnlyList<double> PerClassPrecision { get; set; }

        /// <summary>
        /// per class recall
        /// </summary>
        public IReadOnlyList<double> PerClassRecall { get; set; }

        /// <summary>
        /// counts
        /// </summary>
        public IReadOnlyList<IReadOnlyList<double>> Counts { get; set; }
    }
}
