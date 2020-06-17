using Microsoft.ML.Data;

namespace MLOps.NET.Regression.Entities
{
    public class ModelOutput
    {
        [ColumnName("Score")]
        public float FareAmount;
    }
}
