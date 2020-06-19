using Microsoft.ML;
using Microsoft.ML.Transforms;
using MLOps.NET.Regression.Entities;
using MLOps.NET.SQLite;
using System;
using System.Threading.Tasks;

namespace MLOps.NET.Regression
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            // MLOps: Create experiment and run
            var mlOpsContext = new MLOpsBuilder()
                .UseSQLite(@"C:/MLOps")
                .Build();

            Console.WriteLine("Creating an MLOps Run");
            var runId = await mlOpsContext.CreateRunAsync("Product Category Predictor");
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
            var trainingPipeline = dataProcessingPipeline
                .Append(mlContext.Regression.Trainers.FastTree());

            var trainedModel = trainingPipeline.Fit(testTrainTest.TrainSet);

            Console.WriteLine("Evaluating the model");
            var predictions = trainedModel.Transform(testTrainTest.TestSet);
            var metrics = mlContext.Regression.Evaluate(predictions);

            //MLOps: Log Metrics
            Console.WriteLine("Logging metrics");
            await mlOpsContext.LogMetricsAsync(runId, metrics);

            //Save the model
            mlContext.Model.Save(trainedModel, testTrainTest.TrainSet.Schema, "RegressionClassificationModel.zip");

            //MLOps: Upload artifact/model
            Console.WriteLine("Uploading artifact");
            await mlOpsContext.UploadModelAsync(runId, "RegressionClassificationModel.zip");
        }
    }
}
