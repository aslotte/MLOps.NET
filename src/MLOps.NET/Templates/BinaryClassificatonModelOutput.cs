using System;
using Microsoft.ML.Data;

namespace MLOps.NET.Templates
{
    /// <summary>
    /// 
    /// </summary>
    public class BinaryClassificatonModelOutput
    {
        /// <summary>
        /// 
        /// </summary>
        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public float[] Score { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public float Probability { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Label { get; set; }
    }
}
