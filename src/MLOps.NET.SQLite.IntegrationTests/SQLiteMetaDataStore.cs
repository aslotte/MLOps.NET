using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MLOps.NET.SQLite.IntegrationTests
{
    [TestClass]
    public class SQLiteMetaDataStore
    {
        [TestMethod]
        public async Task CreateExperimentAsync_Always_ReturnsNonEmptyGuidAsync()
        {
            //Arrange
            MLLifeCycleManager mlm = new MLLifeCycleManager();
            var destinationFolder = @"C:\MLOps";
            mlm.UseSQLite(destinationFolder);

            //Act
            var guid = await mlm.CreateExperimentAsync("first experiment");

            //Assert
            Guid.TryParse(guid.ToString(), out var parsedGuid);
            parsedGuid.Should().NotBeEmpty();
        }

        [TestMethod]
        public async Task UploadModelAsync_ValidModelPath_UploadSuccessAsync()
        {
            //Arrange
            MLLifeCycleManager mlm = new MLLifeCycleManager();
            var destinationFolder = @"C:\MLOps";
            mlm.UseSQLite(destinationFolder);
            var guid = Guid.NewGuid();
            var modelPath = @"C:\data\model.zip";
            var modelStoragePath = @"C:\MLOps";
            using var writer = new StreamWriter(modelPath);
            writer.Close();

            //Act
            await mlm.UploadModelAsync(guid, modelPath);

            //Assert
            var fileExists = File.Exists(Path.Combine(modelStoragePath, $"{guid}.zip"));
            fileExists.Should().BeTrue();
        }
    }
}
