using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Storage;
using Moq;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MLOps.NET.SQLite.IntegrationTests
{
    [TestCategory("Integration")]
    [TestClass]
    public class SQLiteMetaDataStoreTests
    {
        [TestMethod]
        public async Task CreateExperimentAsync_Always_ReturnsNonEmptyGuidAsync()
        {
            //Arrange
            IMLOpsContext sut = new MLOpsBuilder()
                .UseModelRepository(new Mock<IModelRepository>().Object)
                .UseSQLite()
                .Build();

            //Act
            var guid = await sut.LifeCycle.CreateExperimentAsync("first experiment");

            //Assert
            Guid.TryParse(guid.ToString(), out var parsedGuid);
            parsedGuid.Should().NotBeEmpty();
        }

        [TestMethod]
        public async Task SetTrainingTimeAsync_SetsTrainingTimeOnRun()
        {
            //Arrange
            var sut = new MLOpsBuilder()
                .UseSQLite()
                .UseModelRepository(new Mock<IModelRepository>().Object)
                .Build();
            var runId = await sut.LifeCycle.CreateRunAsync("Test");

            var expectedTrainingTime = new TimeSpan(0, 5, 0);

            //Act
            await sut.LifeCycle.SetTrainingTimeAsync(runId, expectedTrainingTime);

            //Assert
            var run = sut.LifeCycle.GetRun(runId);
            run.TrainingTime.Should().Be(expectedTrainingTime);
        }

        [TestMethod]
        public void SetTrainingTimeAsync_NoRunProvided_ThrowsException()
        {
            //Arrange
            var sut = new MLOpsBuilder()
                .UseSQLite()
                .UseModelRepository(new Mock<IModelRepository>().Object)
                .Build();

            var expectedTrainingTime = new TimeSpan(0, 5, 0);

            //Act and Assert
            var runId = Guid.NewGuid();
            var expectedMessage = $"The run with id {runId} does not exist";

            Func<Task> func = new Func<Task>(async () => await sut.LifeCycle.SetTrainingTimeAsync(runId, expectedTrainingTime));

            func.Should().Throw<InvalidOperationException>(expectedMessage);
        }

        [TestMethod]
        public async Task CreateRunAsync_WithGitCommitHash_SetsGitCommitHash()
        {
            //Arrange
            var sut = new MLOpsBuilder()
                .UseSQLite()
                .UseModelRepository(new Mock<IModelRepository>().Object)
                .Build();

            var gitCommitHash = "12323239329392";

            //Act
            var runId = await sut.LifeCycle.CreateRunAsync(Guid.NewGuid(), gitCommitHash);

            //Assert
            var run = sut.LifeCycle.GetRun(runId);
            run.GitCommitHash.Should().Be(gitCommitHash);
        }

        [TestMethod]
        public async Task CreateRunAsync_WithoutGitCommitHash_ShouldProvideEmptyGitCommitHash()
        {
            //Arrange
            var sut = new MLOpsBuilder()
                .UseSQLite()
                .UseModelRepository(new Mock<IModelRepository>().Object)
                .Build();

            //Act
            var runId = await sut.LifeCycle.CreateRunAsync(Guid.NewGuid());

            //Assert
            var run = sut.LifeCycle.GetRun(runId);
            run.GitCommitHash.Should().Be(string.Empty);
        }
    }
}
