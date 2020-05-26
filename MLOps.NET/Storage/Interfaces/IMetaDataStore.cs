using MLOps.NET.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MLOps.NET.Storage
{
    internal interface IMetaDataStore
    {
        Task<Experiment> CreateExperimentAsync(Experiment experiment);

        Task<Run> CreateRunAsync(Run run);

        Task LogMetricAsync(Metric run);
    }
}
