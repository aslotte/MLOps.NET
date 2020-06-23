using MLOps.NET.Entities.Entities;
using System;

namespace MLOps.NET.SQLite.Entities
{
    internal sealed class ConfusionMatrixEntity : IConfusionMatrixEntity
    {
        public ConfusionMatrixEntity() { }

        public ConfusionMatrixEntity(Guid runId)
        {
            RunId = runId;
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public Guid RunId { get ; set ; }

        public string SerializedMatrix { get ; set ; }
    }
}
