using Microsoft.ML;
using MLOps.NET.Extensions;
using MLOps.NET.Regression.Entities;
using MLOps.NET.SQLite;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MLOps.NET.Regression
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var stopwatch = new Stopwatch();

            // MLOps: Create experiment and Run
            var mlOpsContext = new MLOpsBuilder()
                .UseSQLite()
                .UseLocalFileModelRepository()
                .Build();

            Console.WriteLine("Creating an MLOps Run");
            var run = await mlOpsContext.LifeCycle.CreateRunAsync("Taxi Fare Predictor");
            Console.WriteLine($"Run created with Id {run.RunId}");

            var mlContext = new MLContext(seed: 1);

            Console.WriteLine("Loading the data");
            var data = mlContext.Data.LoadFromTextFile<ModelInput>("Data/taxi-fare.csv", hasHeader: true, separatorChar: ',');
            var testTrainTest = mlContext.Data.TrainTestSplit(data);

            //MLOps: Log data - e.g. schema, columns, types and a hash to track changes
            await mlOpsContext.Data.LogDataAsync(run.RunId, data);

            Console.WriteLine("Creating a data processing pipeline");
            var dataProcessingPipeline = mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: "FareAmount")
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "VendorIdEncoded", inputColumnName: "VendorId"))
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "RateCodeEncoded", inputColumnName: "RateCode"))
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "PaymentTypeEncoded", inputColumnName: "PaymentType"))
                .Append(mlContext.Transforms.Concatenate("Features", "VendorIdEncoded", "RateCodeEncoded", "PassengerCount", "TripDistance", "PaymentTypeEncoded"));

            Console.WriteLine("Training the model, please stand-by...");
            stopwatch.Start();
            var trainer = mlContext.Regression.Trainers.FastTree();
            var trainingPipeline = dataProcessingPipeline
                .Append(trainer);

            var trainedModel = trainingPipeline.Fit(testTrainTest.TrainSet);

            await mlOpsContext.Training.LogHyperParametersAsync(run.RunId, trainer);
            stopwatch.Stop();

            //MLOps: Training time
            await mlOpsContext.LifeCycle.SetTrainingTimeAsync(run.RunId, stopwatch.Elapsed);
            Console.WriteLine($"Training time:{mlOpsContext.LifeCycle.GetRun(run.RunId).TrainingTime}");

            Console.WriteLine("Evaluating the model");
            var predictions = trainedModel.Transform(testTrainTest.TestSet);
            var metrics = mlContext.Regression.Evaluate(predictions);

            //MLOps: Log Metrics
            Console.WriteLine("Logging metrics");
            await mlOpsContext.Evaluation.LogMetricsAsync(run.RunId, metrics);

            //Save the model
            mlContext.Model.Save(trainedModel, testTrainTest.TrainSet.Schema, "RegressionClassificationModel.zip");

            //MLOps: Upload artifact/model
            Console.WriteLine("Uploading artifact");
            var runArtifact = await mlOpsContext.Model.UploadAsync(run.RunId, "RegressionClassificationModel.zip");

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
