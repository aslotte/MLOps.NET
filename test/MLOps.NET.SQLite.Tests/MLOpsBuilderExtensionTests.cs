using System;
using System.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Storage;
using Moq;

namespace MLOps.NET.SQLite.Tests
{
    [TestCategory("UnitTests")]
    [TestClass]
    public class MLOpsBuilderExtensionTests
    {
        [TestMethod]
        public void UseSqlLiteStorage_ConfiguresEvaluationCatalog()
        {
            //Act
            IMLOpsContext unitUnderTest = new MLOpsBuilder()
                .UseSQLite()
                .UseModelRepository(new Mock<IModelRepository>().Object)
                .Build();

            unitUnderTest.Should().BeOfType<MLOpsContext>("Because the default IMLLifeCycleManager is MLLifeCycleManager");

            //Assert
            unitUnderTest.Evaluation.Should().NotBeNull();
        }


        [TestMethod]
        public void UseSqlLiteStorage_ConfiguresTrainingCatalog()
        {
            //Act
            IMLOpsContext unitUnderTest = new MLOpsBuilder()
                .UseSQLite()
                .UseModelRepository(new Mock<IModelRepository>().Object)
                .Build();

            unitUnderTest.Should().BeOfType<MLOpsContext>("Because the default IMLLifeCycleManager is MLLifeCycleManager");

            //Assert
            unitUnderTest.Training.Should().NotBeNull();
        }

        [TestMethod]
        public void UseSqlLite_ConfiguresLifeCycleCatalog()
        {
            //Act
            IMLOpsContext unitUnderTest = new MLOpsBuilder()
                .UseSQLite()
                .UseModelRepository(new Mock<IModelRepository>().Object)
                .Build();

            unitUnderTest.Should().BeOfType<MLOpsContext>("Because the default IMLLifeCycleManager is MLLifeCycleManager");

            //Assert
            unitUnderTest.LifeCycle.Should().NotBeNull();
        }
    }
}
