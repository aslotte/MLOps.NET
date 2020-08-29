using BenchmarkDotNet.Attributes;
using MLOps.NET.Catalogs;
using MLOps.NET.Storage;
using MLOps.NET.Utilities;
using Moq;
using System.Threading.Tasks;

namespace MLOps.NET.Benchmarks
{
    public class LifeCylceCatalogBenchmarks
    {
        private Mock<IClock> clockMock;
        private Mock<IExperimentRepository> experimentRepositoryMock;
        private Mock<IRunRepository> runRepositoryMock;
        private LifeCycleCatalog sut;

        [GlobalSetup]
        public void Initialize()
        {
            this.clockMock = new Mock<IClock>();
            this.experimentRepositoryMock = new Mock<IExperimentRepository>();
            this.runRepositoryMock = new Mock<IRunRepository>();
            this.sut = new LifeCycleCatalog(experimentRepositoryMock.Object, runRepositoryMock.Object, clockMock.Object);
        }

        [Benchmark]
        public async Task CreateExperimentAsync()
        {
            await sut.CreateExperimentAsync("Benchmark experiment");
        }
    }
}
