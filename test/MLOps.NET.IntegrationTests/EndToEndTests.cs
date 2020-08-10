using FluentAssertions;
using Microsoft.ML;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MLOps.NET.IntegrationTests
{
    public class EndToEndTests : RepositoryTests
    {
        [TestMethod]
        public async Task DeployModel_GivenACompleteRun_ShouldDeployModel()
        {
            //Arrange and Act
            var mlContext = new MLContext(seed: 2);

            var experimentId = await sut.LifeCycle.CreateExperimentAsync("test");
            var runId = await sut.LifeCycle.CreateRunAsync(experimentId);

            var data = mlContext.Data.LoadFromEnumerable(GetSampleDataForTraining());

            var trainer = mlContext.BinaryClassification.Trainers
                .LbfgsLogisticRegression(labelColumnName: "Label", featureColumnName: "Features");

            await sut.Training.LogHyperParametersAsync(runId, trainer);

            var model = trainer.Fit(data);

            var predicitions = model.Transform(data);
            var metrics = mlContext.BinaryClassification.Evaluate(predicitions, labelColumnName: "Label");

            await sut.Evaluation.LogMetricsAsync(runId, metrics);

            await sut.Model.UploadAsync(runId, "");

            var runArtifact = sut.Model.GetRunArtifacts(runId).First();

            var registeredModel = await sut.Model.RegisterModel(experimentId, runArtifact.RunArtifactId, "The MLOps.NET Team");
            var deploymentTarget = await sut.Deployment.CreateDeploymentTargetAsync("Test");
            await sut.Deployment.DeployModelAsync(deploymentTarget, registeredModel, "The MLOps Team");

            var deployment = sut.Deployment.GetDeployments(experimentId).First();
            deploymentTarget = sut.Deployment.GetDeploymentTargets().First();

            //Assert
            registeredModel.Experiment.Should().NotBeNull();
            registeredModel.Run.Should().NotBeNull();
            registeredModel.Run.RunArtifacts.First().Should().NotBeNull();
            registeredModel.Run.Metrics.Should().NotBeNull();
            registeredModel.Run.HyperParameters.Should().NotBeNull();

            deployment.DeploymentTarget.Should().NotBeNull();
            deploymentTarget.Deployments.Should().NotBeNull();

            runArtifact.Should().NotBeNull();
            runArtifact.Run.Should().NotBeNull();
            runArtifact.Run.RunArtifacts.First().Should().NotBeNull();
            runArtifact.Run.Metrics.Should().NotBeNull();
            runArtifact.Run.HyperParameters.Should().NotBeNull();
        }

        private static List<DataPoint> GetSampleDataForTraining()
        {
            return new List<DataPoint>()
            {
                new DataPoint { Features = new float[3] {0, 2, 1} , Label = false },
                new DataPoint { Features = new float[3] {0, 2, 3} , Label = false },
                new DataPoint { Features = new float[3] {0, 2, 4} , Label = true  },
                new DataPoint { Features = new float[3] {0, 2, 1} , Label = false },
                new DataPoint { Features = new float[3] {0, 2, 2} , Label = false },
                new DataPoint { Features = new float[3] {0, 2, 3} , Label = false },
                new DataPoint { Features = new float[3] {0, 2, 4} , Label = true  },
                new DataPoint { Features = new float[3] {1, 0, 0} , Label = true  }
            };
        }
    }
}
