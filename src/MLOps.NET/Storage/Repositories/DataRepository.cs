using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Microsoft.ML.Data;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage.Database;
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

        ///<inheritdoc cref="IDataRepository"/>
        public DataRepository(IDbContextFactory contextFactory)
        {
            this.contextFactory = contextFactory;
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

        ///<inheritdoc cref="IDataRepository"/>
        public async Task LogDataDistribution<T>(Guid runId, IDataView dataView, string columnName) where T:struct
        {
            using var db = this.contextFactory.CreateDbContext();

            var data = db.Data
                .Include(d => d.DataSchema.DataColumns)
                .First(d => d.RunId == runId);

            var dataColumn = data.DataSchema
                .DataColumns
                .FirstOrDefault(c => c.Name == columnName);

            var column = dataView.Schema.First(c => c.Name == columnName);

            List<DataDistribution> list = GetDataDistributionForColumn<T>(dataView, column, dataColumn);

            db.DataDistributions.AddRange(list);

            await db.SaveChangesAsync();
        }

        private List<DataDistribution> GetDataDistributionForColumn<T>(IDataView dataView, DataViewSchema.Column column, Entities.Impl.DataColumn dataColumn) where T : struct
        {
            var values = dataView.GetColumn<T>(column);

            var dataDistributions = new List<DataDistribution>();
            foreach (var item in values)
            {
                if (dataDistributions.Any(d => d.Value == item.ToString()))
                {
                    var distribution = dataDistributions.Single(d => d.Value == item.ToString());
                    distribution.Count++;
                }
                else
                {
                    var distribution = new DataDistribution
                    {
                        DataColumnId = dataColumn.DataColumnId,
                        Value = item.ToString()
                    };
                    dataDistributions.Add(distribution);
                }

            }

            return dataDistributions;
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

            var data = db.Data
                .Include(x => x.DataSchema.DataColumns)
                .FirstOrDefault(x => x.RunId == runId);

            data.DataSchema.DataColumns.ForEach(x => {
                x.Distribution = db.DataDistributions.Where(y => y.DataColumnId == x.DataColumnId).ToList();
            });

            return data;
        }
    }
}
