using Microsoft.ML.Data;

namespace MLOps.NET.MulticlassClassification.Entities
{
    public class ProductInformation
    {
        [LoadColumn(0)]
        public string ProductName { get; set; }

        [LoadColumn(1)]
        public string Category { get; set; }

        [LoadColumn(2)]
        public float Price { get; set; }

        [LoadColumn(3)]
        public string Description { get; set; }

        [LoadColumn(4)]
        public string Brand { get; set; }
    }
}
