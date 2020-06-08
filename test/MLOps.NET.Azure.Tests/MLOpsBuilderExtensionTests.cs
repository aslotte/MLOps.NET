using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using MLOps.NET.Storage;

namespace MLOps.NET.Azure.Tests
{
    [TestClass]
    public class MLOpsBuilderExtensionTests
    {
        [TestMethod]
        public void UseAzureStorage_ConfiguresManager()
        {
            IMLLifeCycleManager lcManager = new MLOpsBuilder().UseAzureStorage("UseDevelopmentStorage=true").Build();

            Assert.IsInstanceOfType(lcManager, typeof(MLLifeCycleManager));
            var metaDataField = typeof(MLLifeCycleManager).GetField("metaDataStore", BindingFlags.Instance | BindingFlags.NonPublic);
            var repositoryField = typeof(MLLifeCycleManager).GetField("modelRepository", BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.IsInstanceOfType(metaDataField.GetValue(lcManager), typeof(StorageAccountMetaDataStore));
            Assert.IsInstanceOfType(repositoryField.GetValue(lcManager), typeof(StorageAccountModelRepository));
        }
    }
}
