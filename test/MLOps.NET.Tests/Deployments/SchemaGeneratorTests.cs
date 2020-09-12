using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Docker;
using MLOps.NET.Docker.Interfaces;
using MLOps.NET.Docker.Settings;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Storage.Deployments;
using Moq;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;

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
            var modelOutput = sut.GenerateDefinition("ModelOutput", MLTaskType.BinaryClassification);

            //Assert
            Assert.IsTrue(modelOutput.Contains("Probability"));
            }
    }
}
