using Microsoft.ML.Data;

namespace MLOps.NET.Clustering.Entities
{
    class ModelInput
    {
        //Iris-setosa - 0, Iris-versicolor - 1, Iris-virginica - 2
        [LoadColumn(0), ColumnName("Label")]
        public float Label { get; set; }

        [LoadColumn(1)]
        public float SepalLengthCm { get; set; }

        [LoadColumn(2)]
        public float SepalWidthCm { get; set; }

        [LoadColumn(3)]
        public float PetalLengthCm { get; set; }

        [LoadColumn(4)]
        public float PetalWidthCm { get; set; }

    }
}
