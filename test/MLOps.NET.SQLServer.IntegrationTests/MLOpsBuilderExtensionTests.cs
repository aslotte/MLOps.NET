﻿using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.SQLServer.IntegrationTests.Constants;
using MLOps.NET.Storage;
using MLOps.NET.Tests.Common.Configuration;
using Moq;

namespace MLOps.NET.SQLServer.Tests
{
    [TestCategory("IntegrationTestSqlServer")]
    [TestClass]
    public class MLOpsBuilderExtensionTests
    {
        private IConfiguration configuration;

        [TestInitialize]
        public void TestInitialize()
        {
            this.configuration = ConfigurationFactory.GetConfiguration();
        }

        [TestMethod]
        public void UseSQLServerStorage_ConfiguresEvaluationCatalog()
        {
            //Act
            IMLOpsContext unitUnderTest = new MLOpsBuilder()
                .UseSQLServer(this.configuration[ConfigurationKeys.ConnectionString])
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
                .UseSQLServer(this.configuration[ConfigurationKeys.ConnectionString])
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
                .UseSQLServer(this.configuration[ConfigurationKeys.ConnectionString])
                .UseModelRepository(new Mock<IModelRepository>().Object)
                .Build();

            unitUnderTest.Should().BeOfType<MLOpsContext>("Because the default IMLLifeCycleManager is MLLifeCycleManager");

            //Assert
            unitUnderTest.LifeCycle.Should().NotBeNull();
        }
    }
}
