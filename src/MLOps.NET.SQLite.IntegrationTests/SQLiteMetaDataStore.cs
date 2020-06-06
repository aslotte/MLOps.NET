using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace MLOps.NET.SQLite.IntegrationTests
{
    [TestClass]
    public class SQLiteMetaDataStore
    {
        [TestMethod]
        public void CreateExperimentAsync_Success()
        {
            //Arrange
            MLLifeCycleManager mlm = new MLLifeCycleManager();
            mlm.UseSQLite();

            //Act
            var guid = mlm.CreateExperimentAsync("first experiment").Result;

            //Assert
            Guid.TryParse(guid.ToString(), out var parsedGuid);
            parsedGuid.Should().NotBeEmpty();
        }

        [TestMethod]
        public void UploadModelAsync_Success()
        {
            //Arrange
            MLLifeCycleManager mlm = new MLLifeCycleManager();
            mlm.UseSQLite();
            var guid = Guid.NewGuid();
            var modelPath = @"C:\data\model.zip";
            var modelStoragePath = @"C:\MLOps";
            using var writer = new StreamWriter(modelPath);
            writer.Close();

            //Act
            mlm.UploadModelAsync(guid, modelPath).Wait();

            //Assert
            var fileExists = File.Exists(Path.Combine(modelStoragePath, $"{guid}.zip"));
            fileExists.Should().BeTrue();
        }
    }
}
