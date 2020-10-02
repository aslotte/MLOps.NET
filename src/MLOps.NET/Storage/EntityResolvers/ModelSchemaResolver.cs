using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace MLOps.NET.Storage.EntityResolvers
{
    internal sealed class ModelSchemaResolver : IEntityResolver<ModelSchema>
    {
        public ModelSchema BuildEntity(IMLOpsDbContext db, ModelSchema entity)
        {
            return entity;
        }

        public List<ModelSchema> BuildEntities(IMLOpsDbContext db, List<ModelSchema> entities)
        {
            return entities.Select(x => BuildEntity(db, x)).ToList();
        }
    }
}
