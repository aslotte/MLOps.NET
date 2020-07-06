using Microsoft.Azure.Cosmos.Table;
using MLOps.NET.Entities.Interfaces;
using System;

namespace MLOps.NET.Azure.Entities
{
    internal sealed class HyperParameter : TableEntity, IHyperParameter
    {
        public HyperParameter() { }

        public HyperParameter(Guid runId, string parameterName, string value)
        {
            PartitionKey = runId.ToString();
            RowKey = ParameterName = parameterName;
            Value = value;
            RunId = runId;
        }

        public string ParameterName { get; set; }

        public string Value { get; set; }

        public Guid RunId { get; set; }
    }
}
