using MLOps.NET.Entities.Interfaces;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MLOps.NET.Entities.Impl
{
    ///<inheritdoc cref="IData"/>
    public sealed class Data : IData
    {
        ///<inheritdoc cref="IData"/>
        public Data(Guid runId)
        {
            Id = Guid.NewGuid();
            RunId = runId;
        }

        ///<inheritdoc cref="IData"/>
        public Guid Id { get; set; }

        ///<inheritdoc cref="IData"/>
        public Guid RunId { get; set; }

        ///<inheritdoc cref="IData"/>
        [NotMapped]
        public IDataSchema DataSchema { get; set; } 
    }
}
