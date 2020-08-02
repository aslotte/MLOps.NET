using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLOps.NET.IntegrationTests
{
    public class DeploymentCatalogTests : RepositoryTests
    {
        [TestMethod]
        public async Task CreateDeploymentTarget_GivenName_CreatesAValidDeploymentTarget()
        {
            //Act
            await sut.Deployment.CreateDeploymentTargetAsync("Production");

            //Assert
            var deploymentTargets = sut.Deployment.GetDeploymentTargets();
            deploymentTargets.First().Name.Should().Be("Production");
            deploymentTargets.First().CreatedDate.Date.Should().Be(DateTime.Now.Date);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "Deployment target name was not specified")]
        public async Task CreateDeploymentTarget_GivenNameIsNullOrEmpty_ThrowsExceptionWithMessage()
        {
            //Act
            await sut.Deployment.CreateDeploymentTargetAsync(null);
        }
    }
}
