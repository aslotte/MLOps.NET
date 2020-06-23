using Microsoft.Azure.Cosmos.Table;
using Microsoft.ML.Calibrators;
using MLOps.NET.Entities;
using MLOps.NET.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MLOps.NET.Azure.Entities
{
    internal sealed class ConfusionMatrixEntity : TableEntity, IConfusionMatrixEntity
    {
        public ConfusionMatrixEntity() { }

        public ConfusionMatrixEntity(Guid runId)
        {
            PartitionKey = nameof(ConfusionMatrix);
            RowKey = runId.ToString();
            RunId = runId;
        }

        public Guid RunId { get; set; }

        public string SerializedMatrix { get; set; }
    }
}
