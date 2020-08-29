using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Docker;
using MLOps.NET.Docker.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MLOps.NET.Tests.Deployments
{
    [TestClass]
    public class DockerContextTests
    {
        private DockerContext sut;

        [TestInitialize]
        public void TestInitialize()
        {
            this.sut = new DockerContext(new CliExecutor(new DockerSettings()), new System.IO.Abstractions.FileSystem(), new DockerSettings());
        }
    }
}
