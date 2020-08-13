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
            var runId = await sut.LifeCycle.CreateRunAsync("test");

            var data = LoadData();

            //Act
            await sut.Data.LogDataAsync(runId, data);

            //Assert
            var savedData = sut.Data.GetData(runId);

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
            var runId = await sut.LifeCycle.CreateRunAsync("test");

            var data = LoadData();

            //Act
            await sut.Data.LogDataAsync(runId, data);

            //Assert
            var savedData = sut.Data.GetData(runId);

            savedData.DataHash.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public async Task LogDataAsync_ShouldGenerateDifferentHashWhenDataChanges()
        {
            var previousRunId = await sut.LifeCycle.CreateRunAsync("previous run");
            var currentRunId = await sut.LifeCycle.CreateRunAsync("current run");
            var data = LoadData();

            var updatedData = LoadUpdatedData();

            //Act
            await sut.Data.LogDataAsync(previousRunId, data);
            await sut.Data.LogDataAsync(currentRunId, updatedData);
            var previousRunData = sut.Data.GetData(previousRunId);
            var currentRunData = sut.Data.GetData(currentRunId);
            //Assert


            currentRunData.DataHash.Should().NotBe(previousRunData.DataHash);
        }

        [TestMethod]
        public async Task LogDataAsync_GivenValidDataView_ShouldLogDataDistribution()
        {
            var runId = await sut.LifeCycle.CreateRunAsync("test");

            var data = LoadData();

            //Act
            await sut.Data.LogDataAsync(runId, data);
            await sut.Data.LogDataDistribution<Boolean>(runId, data, "Sentiment");

            //Assert
            var distributions = sut.Data.GetDataDistribution<Boolean>(runId, data, "Sentiment");

            distributions.Count
                .Should().BeGreaterThan(0);
        }

        private IDataView LoadData()
        {
            var mlContext = new MLContext(seed: 1);
            return mlContext.Data.LoadFromTextFile<ProductReview>("Data/product_reviews.csv", hasHeader: true, separatorChar: ',');
        }

        private IDataView LoadUpdatedData()
        {
            var mlContext = new MLContext(seed: 1);
            return mlContext.Data.LoadFromTextFile<ProductReview>("Data/product_reviews_updated.csv", hasHeader: true, separatorChar: ',');
        }
    }
}
