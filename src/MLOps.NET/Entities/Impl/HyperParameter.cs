using MLOps.NET.Entities.Interfaces;
using System;

namespace MLOps.NET.Entities.Impl
{
    ///<inheritdoc cref="IHyperParameter"/>
    public sealed class HyperParameter : IHyperParameter
    {
        ///<inheritdoc cref="IHyperParameter"/>
        public HyperParameter(Guid runId, string parameterName, string value)
        {
            ParameterName = parameterName;
            Value = value;
            RunId = runId;
            Id = Guid.NewGuid();
        }

        ///<inheritdoc cref="IHyperParameter"/>
        public Guid Id { get; set; }

        ///<inheritdoc cref="IHyperParameter"/>
        public string ParameterName { get; set; }

        ///<inheritdoc cref="IHyperParameter"/>
        public string Value { get; set; }

        ///<inheritdoc cref="IHyperParameter"/>
        public Guid RunId { get; set; }
    }
}
