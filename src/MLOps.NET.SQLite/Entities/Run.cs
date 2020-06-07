using MLOps.NET.Entities.Entities;
using System;

namespace MLOps.NET.SQLite.Entities
{
    internal sealed class Run : IRun
    {
        public Run() { }

        public Run(Guid experimentId)
        {
            Id = Guid.NewGuid();
            RunDate = DateTime.UtcNow;
            ExperimentId = experimentId;
        }

        public Guid Id { get; set; }

        public DateTime RunDate { get; set; }

        public Guid ExperimentId { get; set; }
    }
}
