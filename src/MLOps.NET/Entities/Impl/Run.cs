using MLOps.NET.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MLOps.NET.Entities.Impl
{
    ///<inheritdoc cref="IRun"/>
    public sealed class Run : IRun
    {
        ///<inheritdoc cref="IRun"/>
        public Run() { }

        ///<inheritdoc cref="IRun"/>
        public Run(Guid experimentId)
        {
            Id = Guid.NewGuid();
            RunDate = DateTime.UtcNow;
            ExperimentId = experimentId;
        }

        ///<inheritdoc cref="IRun"/>
        public Guid Id { get; set; }

        ///<inheritdoc cref="IRun"/>
        public DateTime RunDate { get; set; }

        ///<inheritdoc cref="IRun"/>
        public Guid ExperimentId { get; set; }

        ///<inheritdoc cref="IRun"/>
        [NotMapped]
        public List<IMetric> Metrics { get; set; }

        ///<inheritdoc cref="IRun"/>
        public TimeSpan TrainingTime { get; set; }

        ///<inheritdoc cref="IRun"/>
        public string GitCommitHash { get; set; }
    }
}
