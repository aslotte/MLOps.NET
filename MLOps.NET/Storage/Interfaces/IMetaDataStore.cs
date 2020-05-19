using MLOps.NET.Entities;
using System.Threading.Tasks;

namespace MLOps.NET.Storage
{
    internal interface IMetaDataStore
    {
        Task<Experiment> CreateExperimentAsync(Experiment experiment);

        Task<Run> CreateRunAsync(Run run);
    }
}
