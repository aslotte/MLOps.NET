using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using MLOps.NET.Storage;
using System.IO;

namespace MLOps.NET.SQLite.Tests
{
    [TestClass]
    public class MLOpsBuilderExtensionTests
    {
        [TestMethod]
        public void UseAzureStorage_ConfiguresManager()
        {
            var sqlitePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}{Path.DirectorySeparatorChar}.mlops";
            IMLLifeCycleManager lcManager = new MLOpsBuilder().UseSQLite(sqlitePath).Build();

            Assert.IsInstanceOfType(lcManager, typeof(MLLifeCycleManager));
            var metaDataField = typeof(MLLifeCycleManager).GetField("metaDataStore", BindingFlags.Instance | BindingFlags.NonPublic);
            var repositoryField = typeof(MLLifeCycleManager).GetField("modelRepository", BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.IsInstanceOfType(metaDataField.GetValue(lcManager), typeof(SQLiteMetaDataStore));
            Assert.IsInstanceOfType(repositoryField.GetValue(lcManager), typeof(LocalFileModelRepository));
        }
    }
}
