using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Catalogs;
using MLOps.NET.Storage;
using System;
using System.IO;
using System.Reflection;

namespace MLOps.NET.SQLite.Tests
{
    [TestClass]
    public class MLOpsBuilderExtensionTests
    {
        [TestMethod]
        public void UseAzureStorage_ConfiguresEvaluationCatalog()
        {
            //Act
            var sqlitePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}{Path.DirectorySeparatorChar}.mlops";
            IMLOpsContext unitUnderTest = new MLOpsBuilder().UseSQLite(sqlitePath).Build();

            unitUnderTest.Should().BeOfType<MLOpsContext>("Because the default IMLLifeCycleManager is MLLifeCycleManager");

            //Assert
            unitUnderTest.Evaluation.Should().NotBeNull();

            var metaDataField = typeof(EvaluationCatalog).GetField("metaDataStore", BindingFlags.Instance | BindingFlags.NonPublic);
            metaDataField.GetValue(unitUnderTest.Evaluation).Should().BeOfType<SQLiteMetaDataStore>();
        }


        [TestMethod]
        public void UseAzureStorage_ConfiguresTrainingCatalog()
        {
            //Act
            var sqlitePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}{Path.DirectorySeparatorChar}.mlops";
            IMLOpsContext unitUnderTest = new MLOpsBuilder().UseSQLite(sqlitePath).Build();

            unitUnderTest.Should().BeOfType<MLOpsContext>("Because the default IMLLifeCycleManager is MLLifeCycleManager");

            //Assert
            unitUnderTest.Training.Should().NotBeNull();

            var metaDataField = typeof(TrainingCatalog).GetField("metaDataStore", BindingFlags.Instance | BindingFlags.NonPublic);

            metaDataField.GetValue(unitUnderTest.Training).Should().BeOfType<SQLiteMetaDataStore>();
        }

        [TestMethod]
        public void UseAzureStorage_ConfiguresLifeCycleCatalog()
        {
            //Act
            var sqlitePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}{Path.DirectorySeparatorChar}.mlops";
            IMLOpsContext unitUnderTest = new MLOpsBuilder().UseSQLite(sqlitePath).Build();

            unitUnderTest.Should().BeOfType<MLOpsContext>("Because the default IMLLifeCycleManager is MLLifeCycleManager");

            //Assert
            unitUnderTest.LifeCycleCatalog.Should().NotBeNull();

            var metaDataField = typeof(LifeCycleCatalog).GetField("metaDataStore", BindingFlags.Instance | BindingFlags.NonPublic);

            metaDataField.GetValue(unitUnderTest.LifeCycleCatalog).Should().BeOfType<SQLiteMetaDataStore>();
        }

        [TestMethod]
        public void UseAzureStorage_ConfiguresModelCatalog()
        {
            //Act
            var sqlitePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}{Path.DirectorySeparatorChar}.mlops";
            IMLOpsContext unitUnderTest = new MLOpsBuilder().UseSQLite(sqlitePath).Build();

            unitUnderTest.Should().BeOfType<MLOpsContext>("Because the default IMLLifeCycleManager is MLLifeCycleManager");

            //Assert
            unitUnderTest.Model.Should().NotBeNull();

            var repositoryField = typeof(ModelCatalog).GetField("modelRepository", BindingFlags.Instance | BindingFlags.NonPublic);
            repositoryField.GetValue(unitUnderTest.Model).Should().BeOfType<LocalFileModelRepository>();
        }
    }
}
