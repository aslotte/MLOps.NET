using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MLOps.NET.Storage
{
    ///<inheritdoc cref="IModelLabelRepository"/>
    public sealed class ModelLabelRepository : IModelLabelRepository
    {
        private readonly IDbContextFactory contextFactory;

        ///<inheritdoc cref="IModelLabelRepository"/>
        public ModelLabelRepository(IDbContextFactory contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        ///<inheritdoc cref="IModelLabelRepository"/>
        public async Task LogModelLabelAsync(Guid runArtifactId, string LabelName, string LabelValue)
        {
            using (var db = this.contextFactory.CreateDbContext())
            {
                var modelLabel = new ModelLabel(runArtifactId, LabelName, LabelValue);
                await db.ModelLabels.AddAsync(modelLabel);
                await db.SaveChangesAsync();
            }
        }

        ///<inheritdoc cref="IModelLabelRepository"/>
        public List<ModelLabel> GetModelLabels(Guid runArtifactId)
        {
            using (var db = this.contextFactory.CreateDbContext())
            {
                return db.ModelLabels.Where(x => x.RunArtifactId == runArtifactId).ToList();
            }
        }
    }
}
