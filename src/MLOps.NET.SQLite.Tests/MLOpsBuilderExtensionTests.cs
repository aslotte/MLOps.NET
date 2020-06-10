using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using MLOps.NET.Storage;
using System.IO;
using FluentAssertions;

namespace MLOps.NET.SQLite.Tests
{
    [TestClass]
    public class MLOpsBuilderExtensionTests
    {
        [TestMethod]
        public void UseAzureStorage_ConfiguresManager()
        {
            var sqlitePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}{Path.DirectorySeparatorChar}.mlops";
            IMLOpsContext lcManager = new MLOpsBuilder().UseSQLite(sqlitePath).Build();

            lcManager.Should().BeOfType<MLOpsContext>("Because the default IMLLifeCycleManager is MLLifeCycleManager");
            var metaDataField = typeof(MLOpsContext).GetField("metaDataStore", BindingFlags.Instance | BindingFlags.NonPublic);
            var repositoryField = typeof(MLOpsContext).GetField("modelRepository", BindingFlags.Instance | BindingFlags.NonPublic);
            
            metaDataField.GetValue(lcManager).Should().BeOfType<SQLiteMetaDataStore>();
            repositoryField.GetValue(lcManager).Should().BeOfType<LocalFileModelRepository>();
        }
    }
}
