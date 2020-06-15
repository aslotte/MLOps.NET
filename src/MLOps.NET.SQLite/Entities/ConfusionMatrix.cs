using MLOps.NET.Entities.Entities;
using System;

namespace MLOps.NET.SQLite.Entities
{
    internal sealed class ConfusionMatrix : IConfusionMatrix
    {
        public ConfusionMatrix(Guid runId, string value)
        {
            Value = value;
            RunId = runId;
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public string Value { get; set; }
        public Guid RunId { get; set; }
    }
}
