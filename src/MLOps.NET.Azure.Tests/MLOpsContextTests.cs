using System;
using System.Reflection;
using FluentAssertions;
using Microsoft.ML;
using Microsoft.ML.Trainers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Storage;
using Moq;

namespace MLOps.NET.Azure.Tests
{
    [TestClass]
    public class MLOpsContextTests
    {
        [TestMethod]
        public void MLOpsContext_ShouldCallLogHyperParameterIfCorrectInput()
        {
            var mlContext = new MLContext(seed: 2);
            var metaDataStore = new Mock<IMetaDataStore>().Object;
            var opsContext = new Mock<IMLOpsContext>();
            var trainer = mlContext.BinaryClassification.Trainers.LbfgsLogisticRegression(labelColumnName: "Sentiment", featureColumnName: "Features");
            var action = new Action(() => opsContext.Object.LogHyperParametersAsync<LbfgsLogisticRegressionBinaryTrainer>(new Guid(),trainer));
        }
    }
}
