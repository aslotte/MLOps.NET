using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage.Database;
using System;
using System.Threading.Tasks;

namespace MLOps.NET.Storage
{
    ///<inheritdoc cref="IHyperParameterRepository"/>
    public sealed class HyperParameterRepository : IHyperParameterRepository
    {
        private readonly IDbContextFactory contextFactory;

        ///<inheritdoc cref="IHyperParameterRepository"/>
        public HyperParameterRepository(IDbContextFactory contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        ///<inheritdoc cref="IHyperParameterRepository"/>
        public async Task LogHyperParameterAsync(Guid runId, string name, string value)
        {
            using (var db = this.contextFactory.CreateDbContext())
            {
                var hyperParameter = new HyperParameter(runId, name, value);
                await db.HyperParameters.AddAsync(hyperParameter);
                await db.SaveChangesAsync();
            }
        }
    }
}
