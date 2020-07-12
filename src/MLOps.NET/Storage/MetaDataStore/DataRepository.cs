using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage.Database;
using System;
using System.Linq;
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
            using (var db = this.contextFactory.CreateDbContext())
            {
                var data = new Data(runId)
                {
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
