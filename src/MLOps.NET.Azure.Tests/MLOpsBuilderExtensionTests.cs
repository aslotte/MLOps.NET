using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Catalogs;
using MLOps.NET.Storage;
using System.Reflection;

namespace MLOps.NET.Azure.Tests
{
    [TestClass]
    public class MLOpsBuilderExtensionTests
    {
        [TestMethod]
        public void UseAzureStorage_ConfiguresEvaluationCatalog()
        {
            //Act
            IMLOpsContext unitUnderTest = new MLOpsBuilder().UseAzureStorage("UseDevelopmentStorage=true").Build();

            unitUnderTest.Should().BeOfType<MLOpsContext>("Because the default IMLLifeCycleManager is MLLifeCycleManager");

            //Assert
            unitUnderTest.Evaluation.Should().NotBeNull();

            var metaDataField = typeof(EvaluationCatalog).GetField("metaDataStore", BindingFlags.Instance | BindingFlags.NonPublic);
            metaDataField.GetValue(unitUnderTest.Evaluation).Should().BeOfType<StorageAccountMetaDataStore>();
        }

        
        [TestMethod]
        public void UseAzureStorage_ConfiguresTrainingCatalog()
        {
            //Act
            IMLOpsContext unitUnderTest = new MLOpsBuilder().UseAzureStorage("UseDevelopmentStorage=true").Build();

            unitUnderTest.Should().BeOfType<MLOpsContext>("Because the default IMLLifeCycleManager is MLLifeCycleManager");

            //Assert
            unitUnderTest.Training.Should().NotBeNull();

            var metaDataField = typeof(TrainingCatalog).GetField("metaDataStore", BindingFlags.Instance | BindingFlags.NonPublic);

            metaDataField.GetValue(unitUnderTest.Training).Should().BeOfType<StorageAccountMetaDataStore>();
        }

        [TestMethod]
        public void UseAzureStorage_ConfiguresLifeCycleCatalog()
        {
            //Act
            IMLOpsContext unitUnderTest = new MLOpsBuilder().UseAzureStorage("UseDevelopmentStorage=true").Build();

            unitUnderTest.Should().BeOfType<MLOpsContext>("Because the default IMLLifeCycleManager is MLLifeCycleManager");

            //Assert
            unitUnderTest.LifeCycleCatalog.Should().NotBeNull();

            var metaDataField = typeof(LifeCycleCatalog).GetField("metaDataStore", BindingFlags.Instance | BindingFlags.NonPublic);

            metaDataField.GetValue(unitUnderTest.LifeCycleCatalog).Should().BeOfType<StorageAccountMetaDataStore>();
        }

        [TestMethod]
        public void UseAzureStorage_ConfiguresModelCatalog()
        {
            //Act
            IMLOpsContext unitUnderTest = new MLOpsBuilder().UseAzureStorage("UseDevelopmentStorage=true").Build();

            unitUnderTest.Should().BeOfType<MLOpsContext>("Because the default IMLLifeCycleManager is MLLifeCycleManager");

            //Assert
            unitUnderTest.Model.Should().NotBeNull();

            var repositoryField = typeof(ModelCatalog).GetField("modelRepository", BindingFlags.Instance | BindingFlags.NonPublic);
            repositoryField.GetValue(unitUnderTest.Model).Should().BeOfType<StorageAccountModelRepository>();
        }
    }
}
