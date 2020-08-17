using Microsoft.ML;
using Microsoft.ML.Trainers;
using MLOps.NET.Clustering.Entities;
using MLOps.NET.Extensions;
using MLOps.NET.SQLite;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MLOps.NET.Clustering
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // MLOps: Create experiment and run.RunId
            var mlOpsContext = new MLOpsBuilder()
                .UseSQLite()
                .UseLocalFileModelRepository()
                .Build();

            Console.WriteLine("Creating an MLOps run.RunId");
            var run = await mlOpsContext.LifeCycle.CreateRunAsync("Iris Predictor");
            Console.WriteLine($"run.RunId created with Id {run.RunId}");

            var mlContext = new MLContext(seed: 1);

            Console.WriteLine("Loading the data");
            var data = mlContext.Data.LoadFromTextFile<ModelInput>("Data/iris-dataset.csv", hasHeader: true, separatorChar: ',');
            var testTrainTest = mlContext.Data.TrainTestSplit(data);
            var features = new[] { nameof(ModelInput.SepalLengthCm), nameof(ModelInput.SepalWidthCm), nameof(ModelInput.PetalLengthCm), nameof(ModelInput.PetalWidthCm) };

            Console.WriteLine("Creating a data processing pipeline");
            var dataProcessingPipeline = mlContext.Transforms.Concatenate("Features", features)                
                .Append(mlContext.Transforms.NormalizeMinMax("Features"));

            Console.WriteLine("Training the model, please stand-by...");
            var trainer = mlContext.Clustering.Trainers.KMeans(featureColumnName: "Features", numberOfClusters: 3);
            var trainingPipeline = dataProcessingPipeline
                .Append(trainer);

            var trainedModel = trainingPipeline.Fit(testTrainTest.TrainSet);

            await mlOpsContext.Training.LogHyperParametersAsync<KMeansTrainer>(run.RunId, trainer);

            //MLOps: Training time
            await mlOpsContext.LifeCycle.SetTrainingTimeAsync(run.RunId);
            Console.WriteLine($"Training time:{mlOpsContext.LifeCycle.GetRun(run.RunId).TrainingTime}");

            Console.WriteLine("Evaluating the model");
            var predictions = trainedModel.Transform(testTrainTest.TestSet);
            var metrics = mlContext.Clustering.Evaluate(predictions);

            //MLOps: Log Metrics
            Console.WriteLine("Logging metrics");
            await mlOpsContext.Evaluation.LogMetricsAsync(run.RunId, metrics);

            //Save the model
            mlContext.Model.Save(trainedModel, testTrainTest.TrainSet.Schema, "IrisModel.zip");

            //MLOps: Upload artifact/model
            Console.WriteLine("Uploading artifact");
            var runArtifact = await mlOpsContext.Model.UploadAsync(run.RunId, "IrisModel.zip");

            //MLOps: Optional - Register model
            Console.WriteLine("Registering model");
            var registeredModel = await mlOpsContext.Model.RegisterModel(run.ExperimentId, runArtifact.RunArtifactId, "John Doe");

            //MLOps: Optional - Create deployment target
            Console.WriteLine("Creating a deployment target");
            var deploymentTarget = await mlOpsContext.Deployment.CreateDeploymentTargetAsync("Test");

            //MLOps: Optional - Deploy model
            Console.WriteLine("Deploying the model");
            var deployment = await mlOpsContext.Deployment.DeployModelAsync(deploymentTarget, registeredModel, deployedBy: "John Doe");

            //MLOps: Optional - Get the run if you want to vizualize inspect it
            run = mlOpsContext.LifeCycle.GetRun(run.RunId);

            Console.WriteLine($"Model deployed to: {deployment.DeploymentUri}");
        }
    }
}
