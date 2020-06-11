using System;

namespace MLOps.NET.Entities.Entities
{
    /// <summary>
    /// HyperParameters associated with a given training run
    /// </summary>
    public interface IHyperParameter
    {
        /// <summary>
        /// Name of HyperParameter
        /// </summary>
        string ParameterName { get; set; }
        /// <summary>
        /// Parameter value
        /// </summary>
        string Value { get; set; }
        /// <summary>
        /// Unique RunId
        /// </summary>
        Guid RunId { get; set; }
    }
}