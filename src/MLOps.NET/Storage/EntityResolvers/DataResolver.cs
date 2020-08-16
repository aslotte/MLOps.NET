using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace MLOps.NET.Storage.EntityResolvers
{
    internal sealed class DataResolver : IEntityResolver<Data>
    {
        public Data BuildEntity(IMLOpsDbContext db, Data entity)
        {
            db.Entry(entity).Reference(x => x.DataSchema).Load();
            db.Entry(entity.DataSchema).Collection(x => x.DataColumns).Load();

            foreach (var dataColumn in entity.DataSchema.DataColumns)
            {
                db.Entry(dataColumn).Collection(x => x.DataDistributions).Load();
            }

            return entity;
        }

        public List<Data> BuildEntities(IMLOpsDbContext db, List<Data> entities)
        {
            return entities.Select(x => BuildEntity(db, x)).ToList();
        }
    }
}
