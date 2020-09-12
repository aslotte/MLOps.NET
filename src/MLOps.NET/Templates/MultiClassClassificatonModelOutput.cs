using System;
using Microsoft.ML.Data;

namespace MLOps.NET.Templates
{
    /// <summary>
    /// 
    /// </summary>
    public class MultiClassClassificatonModelOutput
    {
        /// <summary>
        /// 
        /// </summary>
        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ColumnName("Score")]
        public float[] Score { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ColumnName("Label")]
        public bool Label { get; set; }
    }
}
