using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage;
using MLOps.NET.Storage.Database;
using MLOps.NET.Storage.Deployments;
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

            this.sut = new RunRepository(contextFactory, clockMock.Object, schemaGenerator, new RunResolver(), new RegisteredModelResolver(), new ModelSchemaResolver());
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

        [TestMethod]
        public async Task RegisterSchema_ShouldGetModelSchema()
        {
            //Arrange
            var (ExperimentId, RunId) = await SetupSeedDate();

            //Act
            await this.sut.RegisterSchema<ModelInput,ModelOutput>(RunId);

            //Assert
            using var db = this.contextFactory.CreateDbContext();
            var schemas = await db.Schemas.Where(s => s.RunId == RunId).ToListAsync();
            var modelInputSchema = schemas.First(s => s.Name == "ModelInput").Value;
            var modelOutputSchema = schemas.First(s => s.Name == "ModelOutput").Value;

            modelInputSchema.Should().Contain("using Microsoft.ML.Data;");
            modelInputSchema.Should().Contain("namespace MLOps.NET.Tests.Common.Data");
            modelInputSchema.Should().Contain("public class ModelInput");
            modelInputSchema.Should().Contain("[LoadColumn(0)]");
            modelInputSchema.Should().Contain("public bool Sentiment;");
            modelInputSchema.Should().Contain("public string Review;");


            modelOutputSchema.Should().Contain("using Microsoft.ML.Data;");
            modelOutputSchema.Should().Contain("namespace MLOps.NET.Tests.Common.Data");
            modelOutputSchema.Should().Contain("public class BinaryClassificationModelOutput");
            modelOutputSchema.Should().Contain("[ColumnName(\"PredictedLabel\")]");
            modelOutputSchema.Should().Contain("public bool Prediction");
            modelOutputSchema.Should().Contain("public float[] Score");
            modelOutputSchema.Should().Contain("public float Probability");
            modelOutputSchema.Should().Contain("public bool Label");
        }
    }
}
