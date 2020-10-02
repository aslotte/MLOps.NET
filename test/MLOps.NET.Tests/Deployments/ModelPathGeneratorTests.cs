using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Services;
using System;
using System.IO;

namespace MLOps.NET.Tests.Deployments
{
    [TestClass]
    public class ModelPathGeneratorTests
    {
        private ModelPathGenerator sut;

        [TestInitialize]
        public void TestInitialize()
        {
            this.sut = new ModelPathGenerator();
        }

        [TestMethod]
        public void GetDeploymentPath_ShouldReturnCorrectDeploymentPath()
        {
            var deploymentTarget = new DeploymentTarget("Test");

            var expectedPath = Path.Combine("ExperimentName", "Test", $"ExperimentName.zip");

            // Act
            var deploymentPath = sut.GetDeploymentPath(deploymentTarget, "ExperimentName");

            // Assert
            deploymentPath.Should().Be(expectedPath);
        }

        [TestMethod]
        public void GetModelName_ShouldReturnCorrectModelName()
        {
            //Arrange
            var runId = Guid.NewGuid();

            // Act
            var modelName = sut.GetModelName(runId);

            // Assert
            modelName.Should().Be($"{runId}.zip");
        }
    }
}
