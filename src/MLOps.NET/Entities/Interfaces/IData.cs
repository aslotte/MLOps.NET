using System;

namespace MLOps.NET.Entities.Interfaces
{
    /// <summary>
    /// Entity holding data information
    /// </summary>
    public interface IData
    {
        /// <summary>
        /// Id of the data
        /// </summary>
        Guid Id { get; set; }

        /// <summary>
        /// Associated RunId
        /// </summary>
        Guid RunId { get; set; }

        /// <summary>
        /// Data schema
        /// </summary>
        IDataSchema DataSchema { get; set; }
    }
}
