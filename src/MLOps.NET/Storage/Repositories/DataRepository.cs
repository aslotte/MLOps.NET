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
using System.Security.Cryptography.X509Certificates;
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
        public async Task LogDataDistribution(Guid runId, IDataView dataView, string columnName)
        {
            using var db = this.contextFactory.CreateDbContext();
            var data = db.Data.AsTracking().Include(d => d.DataSchema.DataColumns).First(d => d.RunId == runId);
            db.Entry(data).State = EntityState.Detached;
            var dataColumn = data.DataSchema.DataColumns.FirstOrDefault(c => c.Name == columnName);
            var column = dataView.Schema.First(c => c.Name == columnName);
            List<DataDistribution> list = GetDataDistributionForColumn(dataView, columnName, column, dataColumn);
            dataColumn.Distribution.AddRange(list);
            db.Entry(data).State = EntityState.Modified;
            await db.SaveChangesAsync();
        }

        private List<DataDistribution> GetDataDistributionForColumn(IDataView dataView, string columnName, DataViewSchema.Column column, Entities.Impl.DataColumn dataColumn)
        {
            // using the GetColumn generic method on the IDataView, which gives all the column values in one shot , Type is only known at runtime.
            var method = typeof(ColumnCursorExtensions).GetMethods().Single(m => m.Name == "GetColumn" &&
                                                                            m.GetParameters()[1].ParameterType == typeof(string));

            var values = (dynamic)method
                        .MakeGenericMethod(column.Type.RawType)
                        .Invoke(dataView, new object[] { dataView, columnName });

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
                    var distribution = new DataDistribution();
                    distribution.DataColumnId = dataColumn.DataColumnId;
                    distribution.Value = item.ToString();
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
                .Include(x => x.DataSchema.DataColumns).ThenInclude(x => x.Distribution)
                .FirstOrDefault(x => x.RunId == runId);
            if (data == null) return null;

            return data;
        }
    }
}
