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
            RunDate = DateTime.Now;
        }

        public Guid Id { get; set; }

        public DateTime RunDate { get; set; }
    }
}
