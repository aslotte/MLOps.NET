using MLOps.NET.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MLOps.NET.Entities.Impl
{
    ///<inheritdoc cref="IExperiment"/>
    public sealed class Experiment : IExperiment
    {
        ///<inheritdoc cref="Experiment"/>
        public Experiment(string experimentName)
        {
            Id = Guid.NewGuid();
            ExperimentName = experimentName;
            CreatedAt = DateTime.UtcNow;
        }

        ///<inheritdoc cref="Experiment"/>
        public Guid Id { get; set; }

        ///<inheritdoc cref="Experiment"/>
        public string ExperimentName { get; set; }

        ///<inheritdoc cref="Experiment"/>
        public DateTime CreatedAt { get; }

        ///<inheritdoc cref="Experiment"/>
        [NotMapped]
        public List<IRun> Runs { get; set; }
    }
}
