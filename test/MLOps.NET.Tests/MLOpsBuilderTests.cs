using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET;
using MLOps.NET.Tests.Mocks.Storage;

namespace MLOps.NET.Tests
{
    [TestClass]
    public class MLOpsBuilderTests
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MLOpsBuilder_ThrowsIfMetaDataStoreConfiguredTwice()
        {
            var metaDataStore = new MockMetaDataStore();
            new MLOpsBuilder().UseMetaDataStore(metaDataStore).UseMetaDataStore(metaDataStore).Build();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MLOpsBuilder_ThrowsIfModelRepositoryConfiguredTwice()
        {
            var repository = new MockModelRepository();
            new MLOpsBuilder().UseModelRepository(repository).UseModelRepository(repository).Build();
        }
    }
}
