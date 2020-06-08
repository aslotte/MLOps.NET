using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Storage;
using Moq;

namespace MLOps.NET.Tests
{
    [TestClass]
    public class MLOpsBuilderTests
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MLOpsBuilder_ThrowsIfMetaDataStoreConfiguredTwice()
        {
            var metaDataStore = new Mock<IMetaDataStore>().Object;
            new MLOpsBuilder().UseMetaDataStore(metaDataStore).UseMetaDataStore(metaDataStore);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MLOpsBuilder_ThrowsIfModelRepositoryConfiguredTwice()
        {
            var repository = new Mock<IModelRepository>().Object;
            new MLOpsBuilder().UseModelRepository(repository).UseModelRepository(repository);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MLOpsBuilder_BuildThrowsIfModelRepositoryNotConfigured()
        {
            var metaDataStore = new Mock<IMetaDataStore>().Object;
            new MLOpsBuilder().UseMetaDataStore(metaDataStore).Build();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MLOpsBuilder_BuildThrowsIfMetaDataStoreNotConfigured()
        {
            var repository = new Mock<IModelRepository>().Object;
            new MLOpsBuilder().UseModelRepository(repository).Build();
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

            Assert.IsInstanceOfType(lcManager, typeof(MLLifeCycleManager));
            var metaDataField = typeof(MLLifeCycleManager).GetField("metaDataStore", BindingFlags.Instance | BindingFlags.NonPublic);
            var repositoryField = typeof(MLLifeCycleManager).GetField("modelRepository", BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.AreSame(metaDataStore, metaDataField.GetValue(lcManager));
            Assert.AreSame(repository, repositoryField.GetValue(lcManager));
        }
    }
}
