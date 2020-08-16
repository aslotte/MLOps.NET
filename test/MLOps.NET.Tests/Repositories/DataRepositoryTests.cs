using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Services.Interfaces;
using MLOps.NET.Storage;
using MLOps.NET.Storage.Database;
using MLOps.NET.Storage.EntityConfiguration;
using MLOps.NET.Storage.EntityResolvers;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MLOps.NET.Tests
{
    [TestCategory("UnitTests")]
    [TestClass]
    public class DataRepositoryTests
    {
        private Mock<IDataCalculator> dataCalculatorMock;
        private DbContextFactory contextFactory;
        private IDataRepository sut;

        [TestInitialize]
        public void Initialize()
        {
            var options = new DbContextOptionsBuilder<MLOpsDbContext>()
                .UseInMemoryDatabase(databaseName: "MLOpsNET")
                .Options;

            this.contextFactory = new DbContextFactory(() => new MLOpsDbContext(options, RelationalEntityConfigurator.OnModelCreating));

            this.dataCalculatorMock = new Mock<IDataCalculator>();

            this.sut = new DataRepository(contextFactory, new DataResolver(), dataCalculatorMock.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "A dataset has not yet been logged for this run. Prior to calling LogDataDistribution please call LogDataAsync")]
        public async Task LogDataDistributions_GivenDataHasNotBeenLogged_ShouldThrowException()
        {
            //Arrange
            var runId = Guid.NewGuid();
            var dataView = new MLContext().Data.LoadFromEnumerable(new List<string>());

            //Act
            await this.sut.LogDataDistribution<bool>(runId, dataView, "Sentiment");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "Unable to log data distributions for the column with name Sentiment as it is not contained in the current data schema")]
        public async Task LogDataDistributions_GivenDataColumnDoesNotExists_ShouldThrowException()
        {
            //Arrange
            using var db = contextFactory.CreateDbContext();

            var experiment = new Experiment("Test");
            var run = new Run(experiment.ExperimentId);
            var data = new Data(run.RunId)
            {
                DataSchema = new DataSchema
                {
                    DataColumns = new List<DataColumn>
                    {
                        new DataColumn
                        {
                            Name = "OtherName"
                        }
                    }
                }
            };

            db.Experiments.Add(experiment);
            db.Runs.Add(run);
            db.Data.Add(data);

            var runId = Guid.NewGuid();
            var dataView = new MLContext().Data.LoadFromEnumerable(new List<string>());

            //Act
            await this.sut.LogDataDistribution<bool>(runId, dataView, "Sentiment");
        }
    }
}
