using Microsoft.Azure.Cosmos.Table;
using Microsoft.ML.Calibrators;
using MLOps.NET.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MLOps.NET.Azure.Entities
{
    internal sealed class ConfusionMatrix : TableEntity, IConfusionMatrix
    {
        public ConfusionMatrix() { }

        public ConfusionMatrix(Guid runId, int numberOfClasses, IReadOnlyList<double> perClassPrecision, IReadOnlyList<double> perClassRecall, IReadOnlyList<IReadOnlyList<double>> counts)
        {
            PartitionKey = nameof(ConfusionMatrix);
            RowKey = runId.ToString();
            RunId = runId;
            NumberOfClasses = numberOfClasses;
            PerClassPrecision = perClassPrecision;
            PerClassRecall = perClassRecall;
            Counts = counts;
        }

        public Guid RunId { get; set; }
        public int NumberOfClasses { get; set; }

        public IReadOnlyList<double> PerClassPrecision { get; set; }

        public IReadOnlyList<double> PerClassRecall { get; set; }

        public IReadOnlyList<IReadOnlyList<double>> Counts { get; set; }

        public string SerializedDetails { get; set; }
    }
}
