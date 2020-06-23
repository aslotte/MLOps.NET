using MLOps.NET.Entities.Entities;
using System;

namespace MLOps.NET.SQLite.Entities
{
    internal sealed class HyperParameter : IHyperParameter
    {
        public HyperParameter(Guid runId, string parameterName, string value)
        {
            ParameterName = parameterName;
            Value = value;
            RunId = runId;
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public string ParameterName { get; set; }

        public string Value { get; set; }

        public Guid RunId { get; set; }
    }
}
