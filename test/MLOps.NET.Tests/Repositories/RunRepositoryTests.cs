using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Services;
using MLOps.NET.Services.Interfaces;
using MLOps.NET.Storage;
using MLOps.NET.Storage.Database;
using MLOps.NET.Storage.EntityConfiguration;
using MLOps.NET.Storage.EntityResolvers;
using MLOps.NET.Tests.Common.Data;
using MLOps.NET.Utilities;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MLOps.NET.Tests
{
    [TestCategory("UnitTests")]
    [TestClass]
    public class RunRepositoryTests
    {
        private Mock<IClock> clockMock;
        private ISchemaGenerator schemaGenerator;
        private DbContextFactory contextFactory;
        private IRunRepository sut;

        [TestInitialize]
        public void Initialize()
        {
            var options = new DbContextOptionsBuilder<MLOpsDbContext>()
                .UseInMemoryDatabase(databaseName: "MLOpsNET")
                .Options;

            this.contextFactory = new DbContextFactory(() => new MLOpsDbContext(options, RelationalEntityConfigurator.OnModelCreating));

            this.clockMock = new Mock<IClock>();
            this.schemaGenerator = new SchemaGenerator();

            this.sut = new Storage.RunRepository(contextFactory, clockMock.Object, schemaGenerator, new RunResolver(), new RegisteredModelResolver());
        }

        [TestMethod]
        public async Task RegisterModel_ShouldSetRegisteredDate()
        {
            //Arrange
            var (ExperimentId, RunId) = await SetupSeedDate();

            var now = DateTime.Now;
            this.clockMock.Setup(x => x.UtcNow).Returns(now);

            //Act
            await this.sut.CreateRegisteredModelAsync(ExperimentId, RunId, "By me", "Model Registered By Test");

            //Assert
            using var db = this.contextFactory.CreateDbContext();
            var registeredModel = db.RegisteredModels.First();

            registeredModel.RegisteredDate.Should().Be(now);
        }

        private async Task<(Guid ExperimentId, Guid RunId)> SetupSeedDate()
        {
            using var db = contextFactory.CreateDbContext();

            var experiment = new Experiment("Test");
            var run = new Run(experiment.ExperimentId);
            var runArtifact = new RunArtifact()
            {
                RunId = run.RunId
            };

            db.Experiments.Add(experiment);
            db.Runs.Add(run);
            db.RunArtifacts.Add(runArtifact);

            await db.SaveChangesAsync();

            return (experiment.ExperimentId, runArtifact.RunId);
        }
    }
}
