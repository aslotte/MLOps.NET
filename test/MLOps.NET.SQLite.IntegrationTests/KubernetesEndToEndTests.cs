using FluentAssertions;
using Microsoft.ML;
using Microsoft.ML.Transforms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Extensions;
using MLOps.NET.SQLite.IntegrationTests.Constants;
using MLOps.NET.SQLite.IntegrationTests.Schema;
using MLOps.NET.Tests.Common.Configuration;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MLOps.NET.SQLite.IntegrationTests
{
    [TestCategory("Integration")]
    [TestClass]
    public class KubernetesEndToEndTests
    {
        private IMLOpsContext sut;

        [TestInitialize]
        public void Initialize()
        {
            var configuration = ConfigurationFactory.GetConfiguration();

            this.sut = new MLOpsBuilder()
                .UseLocalFileModelRepository()
                .UseSQLite()
                .UseContainerRegistry(
                configuration[ConfigurationKeys.AzureRegistryName], configuration[ConfigurationKeys.AzureUsername], configuration[ConfigurationKeys.AzurePassword])
                .UseKubernetes(configuration[ConfigurationKeys.KubeConfig])
                .Build();
        }

        [TestMethod]
        public async Task DeployModel_GivenACompleteRun_ShouldDeployModelToContainer()
        {
            //Arrange and Act
            var mlContext = new MLContext(seed: 2);

            var run = await sut.LifeCycle.CreateRunAsync("titanic");

            var data = mlContext.Data.LoadFromTextFile<ModelInput>("Data/titanic.csv", hasHeader: true, separatorChar: ',');
            var testTrainTest = mlContext.Data.TrainTestSplit(data);

            var dataProcessingPipeline =
                mlContext.Transforms.ReplaceMissingValues(nameof(ModelInput.Age), replacementMode: MissingValueReplacingEstimator.ReplacementMode.Mean)
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(nameof(ModelInput.Sex)))
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(nameof(ModelInput.Pclass)))
                .Append(mlContext.Transforms.Concatenate("Features", nameof(ModelInput.Pclass), nameof(ModelInput.Sex), nameof(ModelInput.Age)));

            var trainer = dataProcessingPipeline.Append(mlContext.BinaryClassification.Trainers
                .LbfgsLogisticRegression(nameof(ModelInput.Survived)));

            await sut.Training.LogHyperParametersAsync(run.RunId, trainer);

            var model = trainer.Fit(testTrainTest.TrainSet);

            mlContext.Model.Save(model, testTrainTest.TrainSet.Schema, "model.zip");
            await sut.Model.UploadAsync(run.RunId, "model.zip");

            var runArtifact = sut.Model.GetRunArtifacts(run.RunId).First();

            var registeredModel = await sut.Model.RegisterModel(run.ExperimentId, runArtifact.RunArtifactId, string.Empty);
            var deploymentTarget = await sut.Deployment.CreateDeploymentTargetAsync("Test");

            var deployment = await sut.Deployment.DeployModelToContainerAsync<ModelInput, ModelOutput>(deploymentTarget, registeredModel, string.Empty);

            //Assert
            var httpClient = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(deployment.DeploymentUri)
            };

            //Add JSON payload here

            var response = await httpClient.SendAsync(request);
            response.IsSuccessStatusCode.Should().BeTrue();

            //Add call to remove namespace or deployent after test is complete, e.g. via CLIExecutor
        }
    }
}
