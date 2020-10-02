using Microsoft.ML.Data;

namespace MLOps.NET.Tests.Common.Data
{
    public class ModelInput
    {
        [LoadColumn(0)]
        public float PassengerId { get; set; }

        [LoadColumn(1), ColumnName("Label")]
        public bool Survived { get; set; }

        [LoadColumn(2)]
        public float Pclass { get; set; }

        [LoadColumn(3)]
        public string Name { get; set; }

        [LoadColumn(4)]
        public string Sex { get; set; }

        [LoadColumn(5)]
        public float Age { get; set; }

        [LoadColumn(6)]
        public float SibSp { get; set; }

        [LoadColumn(7)]
        public float Parch { get; set; }

        [LoadColumn(8)]
        public string Ticket { get; set; }

        [LoadColumn(9)]
        public float Fare { get; set; }

        [LoadColumn(10)]
        public string Cabin { get; set; }

        [LoadColumn(11)]
        public string Embarked { get; set; }
    }
}
