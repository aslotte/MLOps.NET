using Microsoft.ML;
using Microsoft.ML.Trainers;
using MLOps.NET.MulticlassClassification.Entities;
using MLOps.NET.SQLite;
using MLOps.NET.Storage;
using System;
using System.Diagnostics;
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
                .Build();

            Console.WriteLine("Creating an MLOps Run");
            var runId = await mlOpsContext.LifeCycle.CreateRunAsync("Product Category Predictor");
            Console.WriteLine($"Run created with Id {runId}");

            var mlContext = new MLContext(seed: 1);

            Console.WriteLine("Loading the data");
            var data = mlContext.Data.LoadFromTextFile<ProductInformation>("Data/ecommerce.csv", hasHeader: true, separatorChar: ',');
            var testTrainTest = mlContext.Data.TrainTestSplit(data);

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

            await mlOpsContext.Training.LogHyperParametersAsync<SdcaMaximumEntropyMulticlassTrainer>(runId, trainer);
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
        }
    }
}
