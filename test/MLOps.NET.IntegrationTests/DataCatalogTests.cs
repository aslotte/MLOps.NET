using FluentAssertions;
using Microsoft.ML;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var savedData = sut.Data.GetData(runId).FirstOrDefault();

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
            var savedData = sut.Data.GetData(runId).FirstOrDefault();

            savedData.DataHash.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public async Task LogDataAsync_ShouldGenerateDifferentHashWhenDataChanges()
        {
            var runId = await sut.LifeCycle.CreateRunAsync("test");

            var data = LoadData();

            var updatedData = LoadUpdatedData();

            //Act
            await sut.Data.LogDataAsync(runId, data);
            await sut.Data.LogDataAsync(runId, updatedData);
            var allData = sut.Data.GetData(runId);
            //Assert


            allData.First().DataHash.Should().NotBe(allData.Last().DataHash);
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
