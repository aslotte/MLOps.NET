using BenchmarkDotNet.Running;

namespace MLOps.NET.Benchmarks
{
    public class Benchmark
    {
        public static void Main(string[] args)
        {
            SQLiteBenchmarks();
            SQLServerBenchmarks();
            SQLServerAzure();
        }

        public static void SQLiteBenchmarks()
        {
            var lifeCycleCatalogBenchmarks = BenchmarkRunner.Run<LifeCycleCatalogSQLiteBenchmark>();
        }

        public static void SQLServerBenchmarks()
        {
            var lifeCycleCatalogBenchmarks = BenchmarkRunner.Run<LifeCycleCatalogSQLServerBenchmark>();
        }

        public static void SQLServerAzure()
        {
            var lifeCycleCatalogBenchmarks = BenchmarkRunner.Run<LifeCycleCatalogAzureBenchmark>();
        }

    }
}
