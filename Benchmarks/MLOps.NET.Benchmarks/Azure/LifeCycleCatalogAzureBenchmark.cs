﻿using BenchmarkDotNet.Attributes;
using System.Threading.Tasks;

namespace MLOps.NET.Benchmarks
{
    public class LifeCycleCatalogAzureBenchmark
    {
        private IMLOpsContext sut;

        [GlobalSetup]
        public void Initialize()
        {
            sut = BenchmarkSetup.InitializeAzure();
        }

        [Benchmark]
        public async Task CreateRunAsync()
        {
            await sut.LifeCycle.CreateRunAsync("Benchmark Azure");
        }
    }
}
