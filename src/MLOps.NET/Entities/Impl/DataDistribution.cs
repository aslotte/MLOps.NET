using System;
using System.Collections.Generic;

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
        /// RunId
        /// </summary>
        public Guid DataDistributionId { get; set; }

        /// <summary>
        /// DataId
        /// </summary>
        public Guid DataColumnId { get; set; }

        /// <summary>
        /// DataColumns
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// DataColumns
        /// </summary>
        public int Count { get; set; }
    }
}