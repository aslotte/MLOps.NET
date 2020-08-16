using FluentAssertions;
using Microsoft.ML;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MLOps.NET.Tests.Services
{
    [TestClass]
    public class DataCalculatorTests
    {
        private DataCalculator sut;

        [TestInitialize]
        public void Initialize()
        {
            this.sut = new DataCalculator();
        }

        [TestMethod]
        public void CalculateDataDistribution_GivenAColumn_ShouldCreateCorrectNumberOfDistributions()
        {
            //Arrange
            var data = new List<Review>
            {
                new Review { IsPositive = true, Comment = "Great!" },
                new Review { IsPositive = false, Comment = "Aweful!" },
                new Review { IsPositive = true, Comment = "I liked it" }
            };

            var dataView = new MLContext().Data.LoadFromEnumerable(data);

            //Act
            var dataDistributions = this.sut.CalculateDataDistributions<bool>(dataView, nameof(Review.IsPositive), Guid.NewGuid());

            //Assert
            dataDistributions.Should().HaveCount(2);
        }

        [TestMethod]
        public void CalculateDataDistribution_GivenAColumn_ShouldCalculateCorrectNumberOfValuesForEachDistribution()
        {
            //Arrange
            var data = new List<Review>
            {
                new Review { IsPositive = true, Comment = "Great!" },
                new Review { IsPositive = false, Comment = "Aweful!" },
                new Review { IsPositive = true, Comment = "I liked it" }
            };

            var dataView = new MLContext().Data.LoadFromEnumerable(data);

            //Act
            var dataDistributions = this.sut.CalculateDataDistributions<bool>(dataView, nameof(Review.IsPositive), Guid.NewGuid());

            //Assert
            var positiveDistribution = dataDistributions.First(x => x.Value == "True");
            var negativeDistribution = dataDistributions.First(x => x.Value == "False");

            positiveDistribution.Count.Should().Be(2);
            negativeDistribution.Count.Should().Be(1);
        }

        [TestMethod]
        public void CalculateDataDistribution_GivenAColumn_ShouldSetDataColumnId()
        {
            //Arrange
            var data = new List<Review>
            {
                new Review { IsPositive = true, Comment = "Great!" },
                new Review { IsPositive = false, Comment = "Aweful!" },
                new Review { IsPositive = true, Comment = "I liked it" }
            };

            var dataView = new MLContext().Data.LoadFromEnumerable(data);
            var dataColumnId = Guid.NewGuid();

            //Act
            var dataDistributions = this.sut.CalculateDataDistributions<bool>(dataView, nameof(Review.IsPositive), dataColumnId);

            //Assert
            dataDistributions.All(x => x.DataColumnId == dataColumnId).Should().BeTrue();
        }

        [TestMethod]
        public void CalculateDataHash_GivenADataView_ShouldCalculateAHash()
        {
            //Arrange
            var data = new List<Review>
            {
                new Review { IsPositive = true, Comment = "Great!" },
                new Review { IsPositive = false, Comment = "Aweful!" },
                new Review { IsPositive = true, Comment = "I liked it" }
            };

            var dataView = new MLContext().Data.LoadFromEnumerable(data);

            //Act
            var dataHash = this.sut.CalculateDataHash(dataView);

            //Assert
            dataHash.Should().NotBeNullOrEmpty();
        }

        class Review
        {
            public bool IsPositive { get; set; }

            public string Comment { get; set; }
        }
    }
}
