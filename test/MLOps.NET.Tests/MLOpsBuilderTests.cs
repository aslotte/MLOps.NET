using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Storage;
using MLOps.NET.Storage.Database;
using Moq;
using System;

namespace MLOps.NET.Tests
{
    [TestCategory("UnitTests")]
    [TestClass]
    public class MLOpsBuilderTests
    {
        [TestMethod]
        public void MLOpsBuilder_ThrowsIfModelRepositoryConfiguredTwice()
        {
            var repository = new Mock<IModelRepository>().Object;
            var action = new Action(() => new MLOpsBuilder().UseModelRepository(repository).UseModelRepository(repository));
            action.Should().Throw<InvalidOperationException>("Because multiple model repositories can not be configured on the same builder");
        }

        [TestMethod]
        public void MLOpsBuilder_BuildThrowsIfModelRepositoryNotConfigured()
        {
            var dbContextFactory = new Mock<IDbContextFactory>().Object;
            var action = new Action(() => new MLOpsBuilder().UseMetaDataRepositories(dbContextFactory).Build());
            action.Should().Throw<ArgumentNullException>("Because a model repository must be configured before building");
        }

        [TestMethod]
        public void MLOpsBuilder_BuildThrowsIfMetaDataStoreNotConfigured()
        {
            var repository = new Mock<IModelRepository>().Object;
            var action = new Action(() => new MLOpsBuilder().UseModelRepository(repository).Build());
            action.Should().Throw<ArgumentNullException>("Because a meta data store must be configured before building");
        }
    }
}
