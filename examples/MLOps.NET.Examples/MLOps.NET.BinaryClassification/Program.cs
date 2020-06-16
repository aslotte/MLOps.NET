using Microsoft.ML;
using Microsoft.ML.Transforms;
using MLOps.NET.BinaryClassification.Entities;
using MLOps.NET.SQLite;
using System;

namespace MLOps.NET.BinaryClassification
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
            var data = mlContext.Data.LoadFromTextFile<ModelInput>("Data/titanic.csv", hasHeader: true, separatorChar: ',');
            var testTrainTest = mlContext.Data.TrainTestSplit(data);

            var features = new[] { nameof(ModelInput.Pclass), nameof(ModelInput.Sex), nameof(ModelInput.Age), nameof(ModelInput.SibSp), nameof(ModelInput.Parch), nameof(ModelInput.Fare), nameof(ModelInput.Embarked) };

            Console.WriteLine("Creating a data processing pipeline");
            var dataProcessingPipeline =
                mlContext.Transforms.ReplaceMissingValues(nameof(ModelInput.Age), replacementMode: MissingValueReplacingEstimator.ReplacementMode.Mean)
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(nameof(ModelInput.Sex)))
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(nameof(ModelInput.Embarked)))
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(nameof(ModelInput.Pclass)))
                .Append(mlContext.Transforms.Concatenate("Features", features))
                .Append(mlContext.Transforms.NormalizeMinMax("Features"));

            Console.WriteLine("Training the model, please stand-by...");
            var trainingPipeline = dataProcessingPipeline
                .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression("Label", "Features"));

            var trainedModel = trainingPipeline.Fit(testTrainTest.TrainSet);

            Console.WriteLine("Evaluating the model");
            var predictions = trainedModel.Transform(testTrainTest.TestSet);
            var metrics = mlContext.BinaryClassification.Evaluate(predictions);

            //MLOps: Log Metrics
            Console.WriteLine("Logging metrics");
            await mlOpsContext.LogMetricsAsync(runId, metrics);

            //Save the model
            mlContext.Model.Save(trainedModel, testTrainTest.TrainSet.Schema, "BinaryClassificationModel.zip");

            //MLOps: Upload artifact/model
            Console.WriteLine("Uploading artifact");
            await mlOpsContext.UploadModelAsync(runId, "BinaryClassificationModel.zip");
        }
    }
}
