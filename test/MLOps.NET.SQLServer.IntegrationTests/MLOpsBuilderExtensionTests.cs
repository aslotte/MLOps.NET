using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Storage;
using Moq;

namespace MLOps.NET.SQLServer.Tests
{
    [TestCategory("IntegrationTestSqlServer")]
    [TestClass]
    public class MLOpsBuilderExtensionTests
    {
        private const string connectionString = "Server=localhost,1433;Database=MLOpsNET_IntegrationTests;User Id=sa;Password=MLOps4TheWin!;";

        [TestMethod]
        public void UseSQLServerStorage_ConfiguresEvaluationCatalog()
        {
            //Act
            IMLOpsContext unitUnderTest = new MLOpsBuilder()
                .UseSQLServer(connectionString)
                .UseModelRepository(new Mock<IModelRepository>().Object)
                .Build();

            unitUnderTest.Should().BeOfType<MLOpsContext>("Because the default IMLLifeCycleManager is MLLifeCycleManager");

            //Assert
            unitUnderTest.Evaluation.Should().NotBeNull();
        }


        [TestMethod]
        public void UseSQLServerStorage_ConfiguresTrainingCatalog()
        {
            //Act
            IMLOpsContext unitUnderTest = new MLOpsBuilder()
                .UseSQLServer(connectionString)
                .UseModelRepository(new Mock<IModelRepository>().Object)
                .Build();

            unitUnderTest.Should().BeOfType<MLOpsContext>("Because the default IMLLifeCycleManager is MLLifeCycleManager");

            //Assert
            unitUnderTest.Training.Should().NotBeNull();
        }

        [TestMethod]
        public void UseSQLServer_ConfiguresLifeCycleCatalog()
        {
            //Act
            IMLOpsContext unitUnderTest = new MLOpsBuilder()
                .UseSQLServer(connectionString)
                .UseModelRepository(new Mock<IModelRepository>().Object)
                .Build();

            unitUnderTest.Should().BeOfType<MLOpsContext>("Because the default IMLLifeCycleManager is MLLifeCycleManager");

            //Assert
            unitUnderTest.LifeCycle.Should().NotBeNull();
        }
    }
}
