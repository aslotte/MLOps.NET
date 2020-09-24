using Microsoft.ML.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Docker;
using MLOps.NET.Docker.Interfaces;
using MLOps.NET.Docker.Settings;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage.Deployments;
using Moq;
using System.IO;
using System.IO.Abstractions;
using FluentAssertions;
using System.Threading.Tasks;
using MLOps.NET.Tests.Common.Data;

namespace MLOps.NET.Tests.Deployments
{
    [TestClass]
    public class SchemaGeneratorTests
    {
        private SchemaGenerator sut;

        [TestInitialize]
        public void TestInitialize()
        {
            this.sut = new SchemaGenerator();
        }

        [TestMethod]
        public void GenerateDefinition_ShouldGenerateModelOutput()
        {
            //Arrang

            //Act
            var modelOutput = sut.GenerateDefinition<ModelOutput>("BinaryClassificationModelOutput");

            //Assert
            modelOutput.Should().Contain("using Microsoft.ML.Data;");
            modelOutput.Should().Contain("namespace MLOps.NET.Tests.Common.Data");
            modelOutput.Should().Contain("public class BinaryClassificationModelOutput");
            modelOutput.Should().Contain("[ColumnName(\"PredictedLabel\")]");
            modelOutput.Should().Contain("public bool Prediction");
            modelOutput.Should().Contain("public float[] Score");
            modelOutput.Should().Contain("public float Probability");
            modelOutput.Should().Contain("public bool Label");
        }
    }
}
