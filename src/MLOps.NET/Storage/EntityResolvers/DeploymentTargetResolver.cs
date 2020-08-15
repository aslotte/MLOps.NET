using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace MLOps.NET.Storage.EntityResolvers
{
    internal sealed class DeploymentTargetResolver : IEntityResolver<DeploymentTarget>
    {
        public DeploymentTarget BuildEntity(IMLOpsDbContext db, DeploymentTarget entity)
        {
            db.Entry(entity).Collection(x => x.Deployments).Load();

            return entity;
        }

        public List<DeploymentTarget> BuildEntities(IMLOpsDbContext db, List<DeploymentTarget> entities)
        {
            return entities.Select(x => BuildEntity(db, x)).ToList();
        }
    }
}
