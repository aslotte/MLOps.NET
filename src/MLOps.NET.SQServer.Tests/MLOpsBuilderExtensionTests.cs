using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Catalogs;
using MLOps.NET.Storage;
using Moq;
using System.Reflection;

namespace MLOps.NET.SQLServer.Tests
{
    [TestCategory("UnitTests")]
    [TestClass]
    public class MLOpsBuilderExtensionTests
    {
        [TestMethod]
        public void UseSQLServerStorage_ConfiguresEvaluationCatalog()
        {
            //Act
            IMLOpsContext unitUnderTest = new MLOpsBuilder()
                .UseSQLServer("connectionString")
                .UseModelRepository(new Mock<IModelRepository>().Object)
                .Build();

            unitUnderTest.Should().BeOfType<MLOpsContext>("Because the default IMLLifeCycleManager is MLLifeCycleManager");

            //Assert
            unitUnderTest.Evaluation.Should().NotBeNull();

            var metaDataField = typeof(EvaluationCatalog).GetField("metaDataStore", BindingFlags.Instance | BindingFlags.NonPublic);
            metaDataField.GetValue(unitUnderTest.Evaluation).Should().BeOfType<SQLServerMetaDataStore>();
        }


        [TestMethod]
        public void UseSQLServerStorage_ConfiguresTrainingCatalog()
        {
            //Act
            IMLOpsContext unitUnderTest = new MLOpsBuilder()
                .UseSQLServer("connectionString")
                .UseModelRepository(new Mock<IModelRepository>().Object)
                .Build();

            unitUnderTest.Should().BeOfType<MLOpsContext>("Because the default IMLLifeCycleManager is MLLifeCycleManager");

            //Assert
            unitUnderTest.Training.Should().NotBeNull();

            var metaDataField = typeof(TrainingCatalog).GetField("metaDataStore", BindingFlags.Instance | BindingFlags.NonPublic);

            metaDataField.GetValue(unitUnderTest.Training).Should().BeOfType<SQLServerMetaDataStore>();
        }

        [TestMethod]
        public void UseSQLServer_ConfiguresLifeCycleCatalog()
        {
            //Act
            IMLOpsContext unitUnderTest = new MLOpsBuilder()
                .UseSQLServer("connectionString")
                .UseModelRepository(new Mock<IModelRepository>().Object)
                .Build();

            unitUnderTest.Should().BeOfType<MLOpsContext>("Because the default IMLLifeCycleManager is MLLifeCycleManager");

            //Assert
            unitUnderTest.LifeCycle.Should().NotBeNull();

            var metaDataField = typeof(LifeCycleCatalog).GetField("metaDataStore", BindingFlags.Instance | BindingFlags.NonPublic);

            metaDataField.GetValue(unitUnderTest.LifeCycle).Should().BeOfType<SQLServerMetaDataStore>();
        }
    }
}
