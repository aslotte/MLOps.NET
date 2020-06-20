using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using MLOps.NET.Extensions;
using System.IO;
using FluentAssertions;
using MLOps.NET.Catalogs;
using System.Reflection;
using MLOps.NET.Storage;
using Moq;

namespace MLOps.NET.Tests
{
    [TestCategory("UnitTests")]
    [TestClass]
    public class MLOpsBuilderExtensionTests
    {
        [TestMethod]
        public void UseAzureStorage_ConfiguresModelCatalog()
        {
            //Act
            var storagePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}{Path.DirectorySeparatorChar}.mlops";
            IMLOpsContext unitUnderTest = new MLOpsBuilder()
                .UseMetaDataStore(new Mock<IMetaDataStore>().Object)
                .UseLocalFileModelRepository(storagePath)
                .Build();

            unitUnderTest.Should().BeOfType<MLOpsContext>("Because the default IMLLifeCycleManager is MLLifeCycleManager");

            //Assert
            unitUnderTest.Model.Should().NotBeNull();

            var repositoryField = typeof(ModelCatalog).GetField("modelRepository", BindingFlags.Instance | BindingFlags.NonPublic);
            repositoryField.GetValue(unitUnderTest.Model).Should().BeOfType<LocalFileModelRepository>();
        }
    }
}
