using FluentAssertions;
using Microsoft.ML;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Tests.Common.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MLOps.NET.IntegrationTests
{
    public class DataCatalogTests : RepositoryTests
    {
        [TestMethod]
        public async Task LogDataAsync_GivenValidDataView_ShouldLogData()
        {
            var run = await sut.LifeCycle.CreateRunAsync("test");

            var data = LoadData();

            //Act
            await sut.Data.LogDataAsync(run.RunId, data);

            //Assert
            var savedData = sut.Data.GetData(run.RunId);

            savedData.DataSchema.ColumnCount.Should().Be(2);

            savedData.DataSchema.DataColumns
                .Any(x => x.Type == nameof(Boolean) && x.Name == "Sentiment")
                .Should()
                .BeTrue();

            savedData.DataSchema.DataColumns
                .Any(x => x.Type == nameof(String) && x.Name == "Review")
                .Should()
                .BeTrue();
        }

        [TestMethod]
        public async Task LogDataAsync_GivenLogHash()
        {
            var run = await sut.LifeCycle.CreateRunAsync("test");

            var data = LoadData();

            //Act
            await sut.Data.LogDataAsync(run.RunId, data);

            //Assert
            var savedData = sut.Data.GetData(run.RunId);

            savedData.DataHash.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public async Task LogDataAsync_ShouldGenerateDifferentHashWhenDataChanges()
        {
            var previousRun = await sut.LifeCycle.CreateRunAsync("previous run");
            var currentRun = await sut.LifeCycle.CreateRunAsync("current run");
            var data = LoadData();

            var updatedData = LoadUpdatedData();

            //Act
            await sut.Data.LogDataAsync(previousRun.RunId, data);
            await sut.Data.LogDataAsync(currentRun.RunId, updatedData);
            var previousRunData = sut.Data.GetData(previousRun.RunId);
            var currentRunData = sut.Data.GetData(currentRun.RunId);
            //Assert


            currentRunData.DataHash.Should().NotBe(previousRunData.DataHash);
        }

        [TestMethod]
        public async Task LogDataAsync_GivenValidDataView_ShouldLogDataDistribution()
        {
            var run = await sut.LifeCycle.CreateRunAsync("test");

            var data = LoadData();

            //Act
            await sut.Data.LogDataAsync(run.RunId, data);
            await sut.Data.LogDataDistribution<Boolean>(run.RunId, data, "Sentiment");

            //Assert
            var savedData = sut.Data.GetData(run.RunId);

            savedData.DataSchema
                .DataColumns.First(x => x.Name == "Sentiment")
                .DataDistributions
                .Count.Should().Be(2);
        }

        private IDataView LoadData()
        {
            var mlContext = new MLContext(seed: 1);
            return mlContext.Data.LoadFromTextFile<ModelInput>("Data/product_reviews.csv", hasHeader: true, separatorChar: ',');
        }

        private IDataView LoadUpdatedData()
        {
            var mlContext = new MLContext(seed: 1);
            return mlContext.Data.LoadFromTextFile<ModelInput>("Data/product_reviews_updated.csv", hasHeader: true, separatorChar: ',');
        }
    }
}
