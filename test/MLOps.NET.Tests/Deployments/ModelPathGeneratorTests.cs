using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage.Deployments;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
            var registeredModel = new RegisteredModel
            {
                RunId = Guid.NewGuid(),
                Experiment = new Experiment("ExperimentName")
            };

            var deploymentTarget = new DeploymentTarget("Test");

            var expectedPath = Path.Combine("ExperimentName", "Test", $"{registeredModel.RunId}.zip");

            // Act
            var deploymentPath = sut.GetDeploymentPath(deploymentTarget, registeredModel);

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
