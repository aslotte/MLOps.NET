using Microsoft.EntityFrameworkCore;
using MLOps.NET.Extensions;
using MLOps.NET.Storage.Database;
using MLOps.NET.Storage.Interfaces;

namespace MLOps.NET.SQLite.IntegrationTests
{
    internal class IntegrationTestSetup
    {
        internal static IMLOpsContext Initialize()
        {
            return new MLOpsBuilder()
                .UseLocalFileModelRepository()
                .UseSQLite()
                .Build();
        }

        internal static IMLOpsDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder()
                .UseSqlite("Data Source=local.db")
                .Options;

            return new DbContextFactory(() => new MLOpsSQLiteDbContext(options)).CreateDbContext();
        }
    }
}
