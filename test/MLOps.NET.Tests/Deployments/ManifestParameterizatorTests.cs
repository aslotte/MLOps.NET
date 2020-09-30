using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Kubernetes;
using MLOps.NET.Kubernetes.Settings;
using System.IO;
using System.IO.Abstractions;
using System.Reflection;

namespace MLOps.NET.Tests.Deployments
{
    [TestClass]
    public class ManifestParameterizatorTests
    {
        private KubernetesSettings kubernetesSettings;
        private ManifestParameterizator sut;
        private readonly string ExpectedServiceManifest = @"apiVersion: v1
kind: Service
metadata:
  name: testexperiment
  namespace: namespace123
spec:
  type: LoadBalancer
  ports:
  - port: 80
  selector:
    app: testexperiment";

        private readonly string ExpectedDeployManifest = @"apiVersion: apps/v1
kind: Deployment
metadata:
  name: testexperiment
  namespace: namespace
spec:
  replicas: 1
  selector:
    matchLabels:
      app: testexperiment 
  template:
    metadata:
      labels:
        app: testexperiment
    spec:
      containers:
        - name: testexperiment
          image: image-namewithcaps
          resources:
            limits:
              memory: ""500Mi""
              cpu: ""1000m""
          ports:
            - containerPort: 80
      imagePullSecrets:
        - name: mlopsnet";

        [TestInitialize]
        public void TestInitialize()
        {
            this.kubernetesSettings = new KubernetesSettings();

            this.sut = new ManifestParameterizator(new FileSystem(), kubernetesSettings);
        }

        [TestMethod]
        public void ParameterizeServiceManifest_GivenParameters_ShouldCreateValidManifest()
        {
            //Arrange
            var experimentName = "testExperiment";
            var namespaceName = "namespace123";

            //Act
            sut.ParameterizeServiceManifest(experimentName, namespaceName);

            //Assert
            var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filePath = Path.Join(basePath, "service.yml");

            var manifest = File.ReadAllText(filePath);
            manifest.Should().Be(ExpectedServiceManifest);
        }

        [TestMethod]
        public void ParameterizeDeployManifest_GivenParameters_ShouldCreateValidManifest()
        {
            //Arrange
            var experimentName = "testExperiment";
            var imageName = "image-nameWithCaps";

            //Act
            sut.ParameterizeDeploymentManifest(experimentName, imageName, "namespace");

            //Assert
            var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filePath = Path.Join(basePath, "deploy.yml");

            var manifest = File.ReadAllText(filePath);
            manifest.Should().Be(ExpectedDeployManifest);
        }
    }
}
