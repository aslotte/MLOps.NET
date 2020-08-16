using Microsoft.ML;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Services.Interfaces;
using MLOps.NET.Storage.Database;
using MLOps.NET.Storage.EntityResolvers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MLOps.NET.Storage
{
    ///<inheritdoc cref="IDataRepository"/>
    public sealed class DataRepository : IDataRepository
    {
        private readonly IDbContextFactory contextFactory;
        private readonly IEntityResolver<Data> dataResolver;
        private readonly IDataCalculator dataCalculator;

        ///<inheritdoc cref="IDataRepository"/>
        public DataRepository(IDbContextFactory contextFactory,
            IEntityResolver<Data> dataResolver,
            IDataCalculator dataCalculator)
        {
            this.contextFactory = contextFactory;
            this.dataResolver = dataResolver;
            this.dataCalculator = dataCalculator;
        }

        ///<inheritdoc cref="IDataRepository"/>
        public async Task LogDataAsync(Guid runId, IDataView dataView)
        {
            using var db = this.contextFactory.CreateDbContext();

            string hash = this.dataCalculator.CalculateDataHash(dataView);
            var data = new Data(runId)
            {
                DataHash = hash,
                DataSchema = new DataSchema()
                {
                    ColumnCount = dataView.Schema.Count()
                }
            };

            foreach (var column in dataView.Schema)
            {
                var dataColumn = new DataColumn()
                {
                    Name = column.Name,
                    Type = column.Type.ToString()
                };
                data.DataSchema.DataColumns.Add(dataColumn);
            }
            db.Data.Add(data);
            await db.SaveChangesAsync();
        }

        ///<inheritdoc cref="IDataRepository"/>
        public Data GetData(Guid runId)
        {
            using var db = this.contextFactory.CreateDbContext();

            var data = db.Data.FirstOrDefault(x => x.RunId == runId);

            return this.dataResolver.BuildEntity(db, data);
        }

        ///<inheritdoc cref="IDataRepository"/>
        public async Task LogDataDistribution<T>(Guid runId, IDataView dataView, string columnName) where T : struct
        {
            var dataColumnId = this.GetData(runId)
                .DataSchema
                .DataColumns
                .FirstOrDefault(c => c.Name == columnName)
                .DataColumnId;

            using var db = this.contextFactory.CreateDbContext();

            var dataDistributions = this.dataCalculator.CalculateDataDistributions<T>(dataView, columnName, dataColumnId);

            db.DataDistributions.AddRange(dataDistributions);

            await db.SaveChangesAsync();
        }
    }
}
