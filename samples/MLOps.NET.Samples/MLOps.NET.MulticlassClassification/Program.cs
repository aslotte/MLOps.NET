using Microsoft.ML;
using Microsoft.ML.Trainers;
using MLOps.NET.Extensions;
using MLOps.NET.MulticlassClassification.Entities;
using MLOps.NET.SQLite;
using MLOps.NET.Storage;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MLOps.NET.MulticlassClassification
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var stopwatch = new Stopwatch();
            
            // MLOps: Create experiment and run
            var mlOpsContext = new MLOpsBuilder()
                .UseSQLite()
                .UseLocalFileModelRepository()
                .Build();

            Console.WriteLine("Creating an MLOps Run");
            var experimentId = await mlOpsContext.LifeCycle.CreateExperimentAsync("Product Category Predictor");
            var runId = await mlOpsContext.LifeCycle.CreateRunAsync(experimentId);
            Console.WriteLine($"Run created with Id {runId}");

            var mlContext = new MLContext(seed: 1);

            Console.WriteLine("Loading the data");
            var data = mlContext.Data.LoadFromTextFile<ProductInformation>("Data/ecommerce.csv", hasHeader: true, separatorChar: ',');
            var testTrainTest = mlContext.Data.TrainTestSplit(data);

            //MLOps: Log data - e.g. schema, columns, types and a hash to track changes
            await mlOpsContext.Data.LogDataAsync(runId, data);

            Console.WriteLine("Creating a data processing pipeline");
            var dataProcessingPipeline = mlContext.Transforms.Conversion.MapValueToKey(nameof(ProductInformation.Category))
                .Append(mlContext.Transforms.Text.FeaturizeText(nameof(ProductInformation.ProductName)))
                .Append(mlContext.Transforms.Text.FeaturizeText(nameof(ProductInformation.Description)))
                .Append(mlContext.Transforms.Categorical.OneHotHashEncoding(nameof(ProductInformation.Brand))
                .Append(mlContext.Transforms.Concatenate("Features", nameof(ProductInformation.ProductName), nameof(ProductInformation.Description), nameof(ProductInformation.Brand), nameof(ProductInformation.Price))
                .Append(mlContext.Transforms.NormalizeMinMax("Features"))));

            Console.WriteLine("Training the model, please stand-by...");
            stopwatch.Start();
            var trainer = mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy(nameof(ProductInformation.Category), "Features");
            var trainingPipeline = dataProcessingPipeline
                .Append(trainer)
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));
            
            var trainedModel = trainingPipeline.Fit(testTrainTest.TrainSet);

            await mlOpsContext.Training.LogHyperParametersAsync(runId, trainer);
            stopwatch.Stop();

            //MLOps: Training time
            await mlOpsContext.LifeCycle.SetTrainingTimeAsync(runId, stopwatch.Elapsed);
            Console.WriteLine($"Training time(H:M:S:Ms):{mlOpsContext.LifeCycle.GetRun(runId).TrainingTime}");

            Console.WriteLine("Evaluating the model");
            var predictions = trainedModel.Transform(testTrainTest.TestSet);
            var metrics = mlContext.MulticlassClassification.Evaluate(predictions, nameof(ProductInformation.Category));

            //MLOps: Log Metrics
            Console.WriteLine("Logging metrics");
            await mlOpsContext.Evaluation.LogMetricsAsync(runId, metrics);
            await mlOpsContext.Evaluation.LogConfusionMatrixAsync(runId, metrics.ConfusionMatrix);

            //Save the model
            mlContext.Model.Save(trainedModel, testTrainTest.TrainSet.Schema, "MultiClassificationModel.zip");

            //MLOps: Upload artifact/model
            Console.WriteLine("Uploading artifact");
            await mlOpsContext.Model.UploadAsync(runId, "MultiClassificationModel.zip");

            //MLOps: Optional - Register model
            Console.WriteLine("Registering model");
            var runArtifact = mlOpsContext.Model.GetRunArtifacts(runId).First();
            await mlOpsContext.Model.RegisterModel(experimentId, runArtifact.RunArtifactId, "John Doe");
            var registeredModel = mlOpsContext.Model.GetLatestRegisteredModel(experimentId);

            //MLOps: Optional - Create deployment target
            Console.WriteLine("Creating a deployment target");
            await mlOpsContext.Deployment.CreateDeploymentTargetAsync("Test");
            var deploymentTarget = mlOpsContext.Deployment.GetDeploymentTargets().First(x => x.Name == "Test");

            //MLOps: Optional - Deploy model
            Console.WriteLine("Deploying the model");
            var deploymentUri = await mlOpsContext.Deployment.DeployModelAsync(deploymentTarget, registeredModel, deployedBy: "John Doe");

            Console.WriteLine($"Model deployed to: {deploymentUri}");
        }
    }
}
