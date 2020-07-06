using Microsoft.ML;
using Microsoft.ML.Trainers;
using MLOps.NET.Clustering.Entities;
using MLOps.NET.SQLite;
using System;
using System.Threading.Tasks;

namespace MLOps.NET.Clustering
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // MLOps: Create experiment and run
            var mlOpsContext = new MLOpsBuilder()
                .UseSQLite()
                .Build();

            Console.WriteLine("Creating an MLOps Run");
            var runId = await mlOpsContext.LifeCycle.CreateRunAsync("Iris Predictor");
            Console.WriteLine($"Run created with Id {runId}");

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

            await mlOpsContext.Training.LogHyperParametersAsync<KMeansTrainer>(runId, trainer);

            //MLOps: Training time
            await mlOpsContext.LifeCycle.SetTrainingTimeAsync(runId);
            Console.WriteLine($"Training time:{mlOpsContext.LifeCycle.GetRun(runId).TrainingTime}");

            Console.WriteLine("Evaluating the model");
            var predictions = trainedModel.Transform(testTrainTest.TestSet);
            var metrics = mlContext.Clustering.Evaluate(predictions);

            //MLOps: Log Metrics
            Console.WriteLine("Logging metrics");
            await mlOpsContext.Evaluation.LogMetricsAsync(runId, metrics);

            //Save the model
            mlContext.Model.Save(trainedModel, testTrainTest.TrainSet.Schema, "IrisModel.zip");

            //MLOps: Upload artifact/model
            Console.WriteLine("Uploading artifact");
            await mlOpsContext.Model.UploadAsync(runId, "IrisModel.zip");
        }
    }
}
