using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Services;
using System.Linq;

namespace MLOps.NET.Tests.Deployments
{
    [TestClass]
    public class PackageDependencyIdentifierTests
    {
        private PackageDependencyIdentifier sut;

        [TestInitialize]
        public void TestInitialize()
        {
            this.sut = new PackageDependencyIdentifier();
        }

        [TestMethod]
        public void IdentifyPackageDependencies_GivenMLNETDependency_ShouldReturnCorrectNameAndVersion()
        {
            //Act
            var packageDependencies = sut.IdentifyPackageDependencies();

            //Assert
            packageDependencies.All(x => x.Name.StartsWith("Microsoft.ML")).Should().BeTrue();
            packageDependencies.All(x => x.Version == "1.5.4").Should().BeTrue();
        }

        [TestMethod]
        public void IdentifyPackageDependencies_GivenFastTreeDependency_ShouldReturnCorrectNameAndVersion()
        {
            //Act
            var packageDependencies = sut.IdentifyPackageDependencies();

            //Assert
            packageDependencies.Any(x => x.Name == "Microsoft.ML.FastTree" && x.Version == "1.5.4").Should().BeTrue();
        }
    }
}
