using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Microsoft.ML.Data;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage.Database;
using MLOps.NET.Storage.EntityResolvers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace MLOps.NET.Storage
{
    ///<inheritdoc cref="IDataRepository"/>
    public sealed class DataRepository : IDataRepository
    {
        private readonly IDbContextFactory contextFactory;
        private readonly IEntityResolver<Data> dataResolver;

        ///<inheritdoc cref="IDataRepository"/>
        public DataRepository(IDbContextFactory contextFactory, IEntityResolver<Data> dataResolver)
        {
            this.contextFactory = contextFactory;
            this.dataResolver = dataResolver;
        }

        ///<inheritdoc cref="IDataRepository"/>
        public async Task LogDataAsync(Guid runId, IDataView dataView)
        {
            using var db = this.contextFactory.CreateDbContext();

            string hash = GetStringHashFromDataView(dataView);
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

        private string GetStringHashFromDataView(IDataView dataView)
        {
            using SHA256 sha256Hash = SHA256.Create();
            using var stream = new MemoryStream();

            new MLContext().Data.SaveAsBinary(dataView, stream);
            var bytes = sha256Hash.ComputeHash(stream);
            return Convert.ToBase64String(bytes);
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
            var dataColumn = this.GetData(runId)
                .DataSchema
                .DataColumns
                .FirstOrDefault(c => c.Name == columnName);

            using var db = this.contextFactory.CreateDbContext();

            var dataDistributions = this.GetDataDistributionForColumn<T>(dataView, columnName, dataColumn);

            db.DataDistributions.AddRange(dataDistributions);

            await db.SaveChangesAsync();
        }

        private List<DataDistribution> GetDataDistributionForColumn<T>(IDataView dataView, string columnName, DataColumn dataColumn) where T : struct
        {
            var column = dataView.Schema.First(c => c.Name == columnName);

            var allValues = dataView.GetColumn<T>(column);
            var distinctValues = allValues.Distinct();

            var dataDistributions = new List<DataDistribution>();
            foreach (var item in distinctValues)
            {
                var distribution = new DataDistribution
                {
                    DataColumnId = dataColumn.DataColumnId,
                    Value = item.ToString(),
                    Count = allValues.Count(v => v.ToString() == item.ToString())
                };

                dataDistributions.Add(distribution);       
            }
            return dataDistributions;
        }

    }
}
