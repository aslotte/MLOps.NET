using MLOps.NET.Entities.Interfaces;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MLOps.NET.SQLite.Entities
{
    internal sealed class Data : IData
    {
        public Data(Guid runId)
        {
            Id = Guid.NewGuid();
            RunId = runId;
        }

        public Guid Id { get; set; }

        public Guid RunId { get; set; }

        [NotMapped]
        public IDataSchema DataSchema { get; set; } 
    }
}
