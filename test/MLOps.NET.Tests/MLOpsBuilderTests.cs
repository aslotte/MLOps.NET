using System;
using System.Reflection;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Storage;
using Moq;

namespace MLOps.NET.Tests
{
    [TestClass]
    public class MLOpsBuilderTests
    {
        [TestMethod]
        public void MLOpsBuilder_ThrowsIfMetaDataStoreConfiguredTwice()
        {
            var metaDataStore = new Mock<IMetaDataStore>().Object;
            var action = new Action(() => new MLOpsBuilder().UseMetaDataStore(metaDataStore).UseMetaDataStore(metaDataStore));
            action.Should().Throw<InvalidOperationException>("Because multiple meta data stores can not be configured on the same builder");
        }

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
            var metaDataStore = new Mock<IMetaDataStore>().Object;
            var action = new Action(() => new MLOpsBuilder().UseMetaDataStore(metaDataStore).Build());
            action.Should().Throw<ArgumentNullException>("Because a meta data store must be configured before building");
        }

        [TestMethod]
        public void MLOpsBuilder_BuildThrowsIfMetaDataStoreNotConfigured()
        {
            var repository = new Mock<IModelRepository>().Object;
            var action = new Action(() => new MLOpsBuilder().UseModelRepository(repository).Build());
            action.Should().Throw<ArgumentNullException>("Because a model repository must be configured before building");
        }

        [TestMethod]
        public void MLOpsBuilder_BuildCreatesConfiguredLifeCycleManager()
        {
            var metaDataStore = new Mock<IMetaDataStore>().Object;
            var repository = new Mock<IModelRepository>().Object;
            IMLLifeCycleManager lcManager = new MLOpsBuilder()
                .UseMetaDataStore(metaDataStore)
                .UseModelRepository(repository)
                .Build();

            lcManager.Should().BeOfType<MLLifeCycleManager>("Because the default IMLLifeCycleManager is MLLifeCycleManager");
            var metaDataField = typeof(MLLifeCycleManager).GetField("metaDataStore", BindingFlags.Instance | BindingFlags.NonPublic);
            var repositoryField = typeof(MLLifeCycleManager).GetField("modelRepository", BindingFlags.Instance | BindingFlags.NonPublic);

            metaDataStore.Should().BeSameAs(metaDataField.GetValue(lcManager), "Because UseMetaDataStore should set the IMetaDataStore instance via constructor");
            repository.Should().BeSameAs(repositoryField.GetValue(lcManager), "Because UseModelRepository should set the IModelRepository instance via constructor");
        }
    }
}
