using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace MLOps.NET.Storage.EntityResolvers
{
    internal sealed class RegisteredModelResolver : IEntityResolver<RegisteredModel>
    {
        public RegisteredModel BuildEntity(IMLOpsDbContext db, RegisteredModel entity)
        {
            db.Entry(entity).Collection(x => x.Deployments).Load();
            return entity;
        }

        public List<RegisteredModel> BuildEntities(IMLOpsDbContext db, List<RegisteredModel> entities)
        {
            return entities.Select(x => BuildEntity(db, x)).ToList();
        }
    }
}
