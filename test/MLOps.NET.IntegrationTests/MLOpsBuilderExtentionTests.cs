using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Extensions;
using MLOps.NET.Storage.Database;
using Moq;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MLOps.NET.IntegrationTests
{
    [TestCategory("Integration")]
    [TestClass]
    public class MLOpsBuilderExtentionTests
    {
        [TestMethod]
        public async Task UploadModelAsync_ValidModelPath_UploadSuccessAsync()
        {
            //Arrange
            var destinationFolder = @"C:\MLOps";
            IMLOpsContext mlm = new MLOpsBuilder()
                .UseMetaDataRepositories(new Mock<IDbContextFactory>().Object)
                .UseLocalFileModelRepository(destinationFolder)
                .Build();

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
    }
}
