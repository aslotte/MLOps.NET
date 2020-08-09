using System;

namespace MLOps.NET.Entities.Impl
{
    /// <summary>
    /// HyperParameters associated with a given training run
    /// </summary>
    public sealed class HyperParameter
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="runId"></param>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        public HyperParameter(Guid runId, string parameterName, string value)
        {
            HyperParameterId = Guid.NewGuid();
            ParameterName = parameterName;
            Value = value;
            RunId = runId;
        }

        /// <summary>
        /// Id
        /// </summary>
        public Guid HyperParameterId { get; set; }

        /// <summary>
        /// ParameterName
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Run
        /// </summary>
        public Run Run { get; set; }

        /// <summary>
        /// RunId
        /// </summary>
        public Guid RunId { get; set; }
    }
}
