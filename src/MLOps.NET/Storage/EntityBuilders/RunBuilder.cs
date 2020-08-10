using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage.Interfaces;
using System.Linq;

namespace MLOps.NET.Storage.EntityBuilders
{
    internal sealed class RunBuilder : IEntityBuilder<Run>
    {
        public Run BuildEntity(IMLOpsDbContext db, Run entity)
        {
            entity.HyperParameters = db.HyperParameters.Where(x => x.RunId == entity.RunId).ToList();
            entity.Metrics = db.Metrics.Where(x => x.RunId == entity.RunId).ToList();
            entity.ConfusionMatrix = db.ConfusionMatrices.FirstOrDefault(x => x.RunId == entity.RunId);
            entity.RunArtifacts = db.RunArtifacts.Where(x => x.RunId == entity.RunId).ToList();

            return entity;
        }
    }
}
