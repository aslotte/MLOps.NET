using Microsoft.EntityFrameworkCore;
using MLOps.NET.Extensions;
using MLOps.NET.SQLite;
using MLOps.NET.Storage.Database;
using MLOps.NET.Storage.Interfaces;

namespace MLOps.NET.Benchmarks
{
    internal class BenchmarkSetup
    {
        internal static IMLOpsContext Initialize()
        {
            return new MLOpsBuilder()
                .UseLocalFileModelRepository()
                .UseSQLite()
                .Build();
        }
    }
}
