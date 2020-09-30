using System;
using System.Collections.Generic;
using System.Text;

namespace MLOps.NET.SQLite.IntegrationTests.Schema
{
    public class ModelOutput
    {
        public bool PredictedLabel { get; set; }

        public float Score { get; set; }

        public float Probability { get; set; }
    }
}
