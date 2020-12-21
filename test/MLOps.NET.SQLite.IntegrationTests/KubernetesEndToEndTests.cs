using CliWrap;
using CliWrap.Buffered;
using FluentAssertions;
using Microsoft.ML;
using Microsoft.ML.Transforms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Exceptions;
using MLOps.NET.Extensions;
using MLOps.NET.SQLite.IntegrationTests.Constants;
using MLOps.NET.SQLite.IntegrationTests.Schema;
using MLOps.NET.Tests.Common.Configuration;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
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

            var seed = new Random().Next(0, 1000);
            var experimentName = $"titanic-{seed}";

            var run = await sut.LifeCycle.CreateRunAsync(experimentName);

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

            //Act
            var deployment = await sut.Deployment.DeployModelToKubernetesAsync<ModelInput, ModelOutput>(deploymentTarget, registeredModel, string.Empty);

            //Assert
            var response = await CallDeployedApi(deployment);
            response.IsSuccessStatusCode.Should().BeTrue();

            await CleanUpKubernetesResourcesAsync();
        }

        [TestMethod]
        public async Task DeployModel_GivenACompleteRunWithRegisteredSchema_ShouldDeployModelToContainer()
        {
            //Arrange and Act
            var mlContext = new MLContext(seed: 2);

            var run = await sut.LifeCycle.CreateRunAsync("titanic");
            await sut.LifeCycle.RegisterModelSchema<ModelInput, ModelOutput>(run.RunId);

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

            //Act
            var deployment = await sut.Deployment.DeployModelToKubernetesAsync(deploymentTarget, registeredModel, string.Empty);

            //Assert
            var response = await CallDeployedApi(deployment);
            response.IsSuccessStatusCode.Should().BeTrue();

            await CleanUpKubernetesResourcesAsync();
        }

        [ExpectedException(typeof(ModelSchemaNotRegisteredException))]
        [TestMethod]
        public async Task DeployModel_GivenACompleteRunWithoutRegisteredSchema_ShouldThrowException()
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

            //Act
            await sut.Deployment.DeployModelToKubernetesAsync(deploymentTarget, registeredModel, string.Empty);

            //Expects exception
        }

        private async Task<HttpResponseMessage> CallDeployedApi(Deployment deployment)
        {
            var payload = new
            {
                PClass = 3,
                Sex = "male",
                Age = 4
            };

            var json = JsonConvert.SerializeObject(payload);
            var requestMessage = new StringContent(json, Encoding.UTF8, "application/json");
            using var client = new HttpClient();
            return await client.PostAsync(deployment.DeploymentUri, requestMessage);
        }

        private async Task CleanUpKubernetesResourcesAsync()
        {
            var path = SetKubeConfig(ConfigurationFactory.GetConfiguration()[ConfigurationKeys.KubeConfig]);

            await Cli.Wrap("kubectl").WithArguments($"delete ns titanic-test --kubeconfig {path}").ExecuteBufferedAsync();
        }

        private static string SetKubeConfig(string kubeconfigPathOrContent)
        {
            if (Path.IsPathFullyQualified(kubeconfigPathOrContent)) return kubeconfigPathOrContent;

            var kubeConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "kubeconfig");
            File.WriteAllText(kubeConfigPath, kubeconfigPathOrContent);

            return kubeConfigPath;
        }
    }
}
