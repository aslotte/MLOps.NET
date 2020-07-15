using System;

namespace MLOps.NET.Entities.Impl
{
    /// <summary>
    /// Entity holding data information
    /// </summary>
    public sealed class Data
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="runId"></param>
        public Data(Guid runId)
        {
            DataId = Guid.NewGuid();
            RunId = runId;
        }

        /// <summary>
        /// Id
        /// </summary>
        public Guid DataId { get; set; }

        /// <summary>
        /// RunId
        /// </summary>
        public Guid RunId { get; set; }

        /// <summary>
        /// DataSchema
        /// </summary>
        public DataSchema DataSchema { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DataHash { get; set; }
    }
}
