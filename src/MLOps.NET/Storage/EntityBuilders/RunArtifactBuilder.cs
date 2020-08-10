using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage.Interfaces;
using System.Linq;

namespace MLOps.NET.Storage.EntityBuilders
{
    internal sealed class RunArtifactBuilder : IEntityBuilder<RunArtifact>
    {
        private readonly IEntityBuilder<Run> runBuilder;

        public RunArtifactBuilder()
        {
            this.runBuilder = new RunBuilder();
        }
        public RunArtifact BuildEntity(IMLOpsDbContext db, RunArtifact entity)
        {
            if (entity.Run == null)
            {
                entity.Run = db.Runs.First(x => x.RunId == entity.RunId);
                this.runBuilder.BuildEntity(db, entity.Run);
            }
            return entity;
        }
    }
}
