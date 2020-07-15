using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace MLOps.NET.IntegrationTests
{
    [TestClass]
    public class ModelCatalogTests : RepositoryTests
    {
        [TestMethod]
        public async Task UploadModelAsync_ShouldCreateRunArtifact()
        {
            //Arrange
            var experimentId = await sut.LifeCycle.CreateExperimentAsync("test");
            var runId = await sut.LifeCycle.CreateRunAsync(experimentId);

            //Act
            await sut.Model.UploadAsync(runId, "");

            //Assert
            var runArtifacts = sut.Model.GetRunArtifacts(runId);
            runArtifacts.First().RunId.Should().Be(runId);
            runArtifacts.First().Name.Should().Be($"{runId}.zip");
        }
    }
}
