using Microsoft.ML;
using MLOps.NET.Extensions;
using MLOps.NET.MulticlassClassification.Entities;
using MLOps.NET.SQLite;
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
            var run = await mlOpsContext.LifeCycle.CreateRunAsync("Product Category Predictor");
            Console.WriteLine($"Run created with Id {run.RunId}");

            var mlContext = new MLContext(seed: 1);

            Console.WriteLine("Loading the data");
            var data = mlContext.Data.LoadFromTextFile<ProductInformation>("Data/ecommerce.csv", hasHeader: true, separatorChar: ',');
            var testTrainTest = mlContext.Data.TrainTestSplit(data);

            //MLOps: Log data - e.g. schema, columns, types and a hash to track changes
            await mlOpsContext.Data.LogDataAsync(run.RunId, data);

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

            await mlOpsContext.Training.LogHyperParametersAsync(run.RunId, trainer);
            stopwatch.Stop();

            //MLOps: Training time
            await mlOpsContext.LifeCycle.SetTrainingTimeAsync(run.RunId, stopwatch.Elapsed);
            Console.WriteLine($"Training time(H:M:S:Ms):{mlOpsContext.LifeCycle.GetRun(run.RunId).TrainingTime}");

            Console.WriteLine("Evaluating the model");
            var predictions = trainedModel.Transform(testTrainTest.TestSet);
            var metrics = mlContext.MulticlassClassification.Evaluate(predictions, nameof(ProductInformation.Category));

            //MLOps: Log Metrics
            Console.WriteLine("Logging metrics");
            await mlOpsContext.Evaluation.LogMetricsAsync(run.RunId, metrics);
            await mlOpsContext.Evaluation.LogConfusionMatrixAsync(run.RunId, metrics.ConfusionMatrix);

            //Save the model
            mlContext.Model.Save(trainedModel, testTrainTest.TrainSet.Schema, "MultiClassificationModel.zip");

            //MLOps: Upload artifact/model
            Console.WriteLine("Uploading artifact");
            var runArtifact = await mlOpsContext.Model.UploadAsync(run.RunId, "MultiClassificationModel.zip");

            //MLOps: Optional - Register model
            Console.WriteLine("Registering model");
            var registeredModel = await mlOpsContext.Model.RegisterModel(run.ExperimentId, runArtifact.RunArtifactId, "John Doe");

            //MLOps: Optional - Create deployment target
            Console.WriteLine("Creating a deployment target");
            var deploymentTarget = await mlOpsContext.Deployment.CreateDeploymentTargetAsync("Test");

            //MLOps: Optional - Deploy model
            Console.WriteLine("Deploying the model");
            var deployment = await mlOpsContext.Deployment.DeployModelToUriAsync(deploymentTarget, registeredModel, deployedBy: "John Doe");

            //MLOps: Optional - Get the run if you want to vizualize inspect it
            run = mlOpsContext.LifeCycle.GetRun(run.RunId);

            Console.WriteLine($"Model deployed to: {deployment.DeploymentUri}");
        }
    }
}
