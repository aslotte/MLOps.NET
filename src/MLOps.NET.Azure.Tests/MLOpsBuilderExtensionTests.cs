using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using MLOps.NET.Storage;
using FluentAssertions;

namespace MLOps.NET.Azure.Tests
{
    [TestClass]
    public class MLOpsBuilderExtensionTests
    {
        [TestMethod]
        public void UseAzureStorage_ConfiguresManager()
        {
            IMLOpsContext lcManager = new MLOpsBuilder().UseAzureStorage("UseDevelopmentStorage=true").Build();

            lcManager.Should().BeOfType<MLOpsContext>("Because the default IMLLifeCycleManager is MLLifeCycleManager");
            var metaDataField = typeof(MLOpsContext).GetField("metaDataStore", BindingFlags.Instance | BindingFlags.NonPublic);
            var repositoryField = typeof(MLOpsContext).GetField("modelRepository", BindingFlags.Instance | BindingFlags.NonPublic);

            metaDataField.GetValue(lcManager).Should().BeOfType<StorageAccountMetaDataStore>();
            repositoryField.GetValue(lcManager).Should().BeOfType<StorageAccountModelRepository>();
        }
    }
}
