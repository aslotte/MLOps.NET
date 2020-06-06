using System;
using System.Collections.Generic;

namespace MLOps.NET.Entities.Entities
{
    public interface IExperiment
    {
        string ExperimentName { get; set; }
        Guid Id { get; set; }
    }
}