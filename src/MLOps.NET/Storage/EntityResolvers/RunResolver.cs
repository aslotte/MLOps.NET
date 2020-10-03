using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace MLOps.NET.Storage.EntityResolvers
{
    internal sealed class RunResolver : IEntityResolver<Run>
    {
        public Run BuildEntity(IMLOpsDbContext db, Run entity)
        {
            db.Entry(entity).Collection(x => x.HyperParameters).Load();
            db.Entry(entity).Collection(x => x.Metrics).Load();
            db.Entry(entity).Reference(x => x.ConfusionMatrix).Load();
            db.Entry(entity).Collection(x => x.RunArtifacts).Load();
            db.Entry(entity).Collection(x => x.ModelSchemas).Load();
            db.Entry(entity).Collection(x => x.PackageDepedencies).Load();

            return entity;
        }

        public List<Run> BuildEntities(IMLOpsDbContext db, List<Run> entities)
        {
            return entities.Select(x => BuildEntity(db, x)).ToList();
        }
    }
}
