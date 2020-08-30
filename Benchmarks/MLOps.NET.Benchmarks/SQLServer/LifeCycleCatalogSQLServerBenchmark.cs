using BenchmarkDotNet.Attributes;
using System.Threading.Tasks;

namespace MLOps.NET.Benchmarks
{
    public class LifeCycleCatalogSQLServerBenchmark
    {
        private IMLOpsContext sut;

        [GlobalSetup]
        public void Initialize()
        {
            sut = BenchmarkSetup.InitializeSQLServer();
        }

        [Benchmark]
        public async Task CreateRunAsync()
        {
            await sut.LifeCycle.CreateRunAsync("Benchmark SQLServer");
        }
    }
}
