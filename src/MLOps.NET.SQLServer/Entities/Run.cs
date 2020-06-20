using MLOps.NET.Entities.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MLOps.NET.SQLServer.Entities
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

        [NotMapped]
        public List<IMetric> Metrics { get; set; }

        public TimeSpan TrainingTime { get; set; }

        public string GitCommitHash { get; set; }
    }
}
