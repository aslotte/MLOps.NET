using MLOps.NET.Entities;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage.Database;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MLOps.NET.Storage
{
    ///<inheritdoc cref="IConfusionMatrixRepository"/>
    public sealed class ConfusionMatrixRepository : IConfusionMatrixRepository
    {
        private readonly IDbContextFactory contextFactory;

        ///<inheritdoc cref="IConfusionMatrixRepository"/>
        public ConfusionMatrixRepository(IDbContextFactory contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        ///<inheritdoc cref="IConfusionMatrixRepository"/>
        public async Task LogConfusionMatrixAsync(Guid runId, ConfusionMatrix confusionMatrix)
        {
            using (var db = this.contextFactory.CreateDbContext())
            {
                var conMatrix = new ConfusionMatrixEntity(runId)
                {
                    SerializedMatrix = JsonConvert.SerializeObject(confusionMatrix)
                };

                await db.ConfusionMatrices.AddAsync(conMatrix);
                await db.SaveChangesAsync();
            }
        }

        ///<inheritdoc cref="IConfusionMatrixRepository"/>
        public ConfusionMatrix GetConfusionMatrix(Guid runId)
        {
            using (var db = this.contextFactory.CreateDbContext())
            {
                var confusionMatrixEntity = db.ConfusionMatrices.SingleOrDefault(x => x.RunId == runId);

                if (confusionMatrixEntity == null) return null;

                return JsonConvert.DeserializeObject<ConfusionMatrix>(confusionMatrixEntity.SerializedMatrix);
            }
        }
    }
}
