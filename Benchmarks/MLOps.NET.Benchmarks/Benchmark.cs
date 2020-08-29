using BenchmarkDotNet.Running;
using System;

namespace MLOps.NET.Benchmarks
{
    public class Benchmark
    {
        public static void Main(string[] args)
        {
            var lifeCylceCatalogBenchmarks = BenchmarkRunner.Run<LifeCylceCatalogBenchmarks>();
        }
    }
}
