using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MLOps.NET.Tests.Common.Data
{
    public class ModelOutput
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
