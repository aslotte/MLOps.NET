using Microsoft.Azure.Cosmos.Table;
using MLOps.NET.Entities.Entities;
using System;

namespace MLOps.NET.Azure.Entities
{
    internal sealed class ConfusionMatrix : TableEntity, IConfusionMatrix
    {
        public ConfusionMatrix() { }

        public ConfusionMatrix(Guid runId, string value)
        {
            PartitionKey = nameof(ConfusionMatrix);
            RowKey = runId.ToString();
            Value = value;
            RunId = runId;
        }

        public string Value { get; set; }
        public Guid RunId { get; set; }
    }
}
