using System;

namespace MLOps.NET.Entities.Impl
{
    /// <summary>
    /// Entity describing the data schema
    /// </summary>
    public sealed class DataDistribution
    {
        /// <summary>
        /// Ctr
        /// </summary>
        public DataDistribution()
        {
            DataDistributionId = Guid.NewGuid();
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="dataColumnId"></param>
        public DataDistribution(Guid dataColumnId)
        {
            DataDistributionId = Guid.NewGuid();
            DataColumnId = dataColumnId;
        }

        /// <summary>
        /// DataDistributionId
        /// </summary>
        public Guid DataDistributionId { get; set; }

        /// <summary>
        /// DataColumnId
        /// </summary>
        public Guid DataColumnId { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Count
        /// </summary>
        public int Count { get; set; }
    }
}