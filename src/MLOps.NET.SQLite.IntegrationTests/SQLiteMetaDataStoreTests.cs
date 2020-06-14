using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var destinationFolder = @"C:\MLOps";
            IMLOpsContext mlm = new MLOpsBuilder().UseSQLite(destinationFolder).Build();

            //Act
            var guid = await mlm.LifeCycle.CreateExperimentAsync("first experiment");

            //Assert
            Guid.TryParse(guid.ToString(), out var parsedGuid);
            parsedGuid.Should().NotBeEmpty();
        }

        [TestMethod]
        public async Task UploadModelAsync_ValidModelPath_UploadSuccessAsync()
        {
            //Arrange
            var destinationFolder = @"C:\MLOps";
            IMLOpsContext mlm = new MLOpsBuilder().UseSQLite(destinationFolder).Build();
            var guid = Guid.NewGuid();
            var modelPath = @"C:\data\model.zip";
            var modelStoragePath = @"C:\MLOps";
            using var writer = new StreamWriter(modelPath);
            writer.Close();

            //Act
            await mlm.Model.UploadAsync(guid, modelPath);

            //Assert
            var fileExists = File.Exists(Path.Combine(modelStoragePath, $"{guid}.zip"));
            fileExists.Should().BeTrue();
        }

        [TestMethod]
        public async Task SetTrainingTimeAsync_SetsTrainingTimeOnRun()
        {
            //Arrange
            var unitUnderTest = new MLOpsBuilder().UseSQLite().Build();
            var runId = await unitUnderTest.LifeCycle.CreateRunAsync("Test");

            var expectedTrainingTime = new TimeSpan(0, 5, 0);

            //Act
            await unitUnderTest.LifeCycle.SetTrainingTimeAsync(runId, expectedTrainingTime);

            //Assert
            var run = unitUnderTest.LifeCycle.GetRun(runId);
            run.TrainingTime.Should().Be(expectedTrainingTime);
        }

        [TestMethod]
        public void SetTrainingTimeAsync_NoRunProvided_ThrowsException()
        {
            //Arrange
            var unitUnderTest = new MLOpsBuilder().UseSQLite().Build();

            var expectedTrainingTime = new TimeSpan(0, 5, 0);

            //Act and Assert
            var runId = Guid.NewGuid();
            var expectedMessage = $"The run with id {runId} does not exist";

            Func<Task> func = new Func<Task>(async () => await unitUnderTest.LifeCycle.SetTrainingTimeAsync(runId, expectedTrainingTime));

            func.Should().Throw<InvalidOperationException>(expectedMessage);
        }
    }
}
