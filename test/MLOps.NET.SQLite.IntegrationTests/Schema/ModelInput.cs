using Microsoft.ML.Data;

namespace MLOps.NET.SQLite.IntegrationTests.Schema
{
    public class ModelInput
    {
        [LoadColumn(1)]
        public bool Survived { get; set; }

        [LoadColumn(2)]
        public float Pclass { get; set; }

        [LoadColumn(4)]
        public string Sex { get; set; }

        [LoadColumn(5)]
        public float Age { get; set; }
    }
}
