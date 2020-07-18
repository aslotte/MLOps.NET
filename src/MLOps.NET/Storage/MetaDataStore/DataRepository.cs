using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage.Database;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;

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
            using (var db = this.contextFactory.CreateDbContext())
            {
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
        }

        private string GetStringHashFromDataView(IDataView dataView)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                using (var stream = new MemoryStream())
                {
                    new MLContext().Data.SaveAsBinary(dataView, stream);
                    var bytes = sha256Hash.ComputeHash(stream);
                    return Convert.ToBase64String(bytes);
                }
            }
        }

        ///<inheritdoc cref="IDataRepository"/>
        public Data GetData(Guid runId)
        {
            using (var db = this.contextFactory.CreateDbContext())
            {
                var data = db.Data
                    .Include(x => x.DataSchema.DataColumns)
                    .FirstOrDefault(x => x.RunId == runId);
                if (data == null) return null;

                return data;
            }
        }
    }
}
