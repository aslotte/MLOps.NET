using BenchmarkDotNet.Attributes;
using MLOps.NET.Extensions;
using MLOps.NET.SQLite;
using System.Threading.Tasks;

namespace MLOps.NET.Benchmarks
{
    public class LifeCycleCatalogSQLiteBenchmark
    {
        private IMLOpsContext sut;

        [GlobalSetup]
        public void Initialize()
        {
            sut = BenchmarkSetup.InitializeSQLite();
        }

        [Benchmark]
        public async Task CreateRunAsync()
        {
            await sut.LifeCycle.CreateRunAsync("Benchmark SQLite");
        }
    }
}
