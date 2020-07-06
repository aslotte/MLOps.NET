using Microsoft.ML;
using Microsoft.ML.Trainers.FastTree;
using Microsoft.ML.Transforms;
using MLOps.NET.Regression.Entities;
using MLOps.NET.SQLite;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MLOps.NET.Regression
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var stopwatch = new Stopwatch();

            // MLOps: Create experiment and run
            var mlOpsContext = new MLOpsBuilder()
                .UseSQLite()
                .Build();

            Console.WriteLine("Creating an MLOps Run");
            var runId = await mlOpsContext.LifeCycle.CreateRunAsync("Taxi Fare Predictor");
            Console.WriteLine($"Run created with Id {runId}");

            var mlContext = new MLContext(seed: 1);

            Console.WriteLine("Loading the data");
            var data = mlContext.Data.LoadFromTextFile<ModelInput>("Data/taxi-fare.csv", hasHeader: true, separatorChar: ',');
            var testTrainTest = mlContext.Data.TrainTestSplit(data);

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

            await mlOpsContext.Training.LogHyperParametersAsync<FastTreeRegressionTrainer>(runId, trainer);
            stopwatch.Stop();

            //MLOps: Training time
            await mlOpsContext.LifeCycle.SetTrainingTimeAsync(runId, stopwatch.Elapsed);
            Console.WriteLine($"Training time:{mlOpsContext.LifeCycle.GetRun(runId).TrainingTime}");

            Console.WriteLine("Evaluating the model");
            var predictions = trainedModel.Transform(testTrainTest.TestSet);
            var metrics = mlContext.Regression.Evaluate(predictions);

            //MLOps: Log Metrics
            Console.WriteLine("Logging metrics");
            await mlOpsContext.Evaluation.LogMetricsAsync(runId, metrics);

            //Save the model
            mlContext.Model.Save(trainedModel, testTrainTest.TrainSet.Schema, "RegressionClassificationModel.zip");

            //MLOps: Upload artifact/model
            Console.WriteLine("Uploading artifact");
            await mlOpsContext.Model.UploadAsync(runId, "RegressionClassificationModel.zip");
        }
    }
}
