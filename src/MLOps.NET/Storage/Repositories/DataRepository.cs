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
            if (data == null) return data;

            return this.dataResolver.BuildEntity(db, data);
        }

        ///<inheritdoc cref="IDataRepository"/>
        public async Task LogDataDistribution<T>(Guid runId, IDataView dataView, string columnName) where T : struct
        {
            var data = this.GetData(runId);

            if (data == null)
            {
                throw new InvalidOperationException($"A dataset has not yet been logged for this run. Prior to calling {nameof(this.LogDataDistribution)} please call {nameof(this.LogDataAsync)}");
            }

            var dataColumn = data.DataSchema
                .DataColumns
                .FirstOrDefault(c => c.Name == columnName);

            if (dataColumn == null)
            {
                throw new InvalidOperationException($"Unable to log data distributions for the column with name {columnName} as it is not contained in the current data schema");
            }

            using var db = this.contextFactory.CreateDbContext();

            var dataDistributions = this.dataCalculator.CalculateDataDistributions<T>(dataView, columnName, dataColumn.DataColumnId);

            db.DataDistributions.AddRange(dataDistributions);

            await db.SaveChangesAsync();
        }
    }
}
