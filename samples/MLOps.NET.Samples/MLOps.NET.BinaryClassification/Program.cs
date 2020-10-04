using Microsoft.ML;
using Microsoft.ML.Transforms;
using MLOps.NET.BinaryClassification.Entities;
using MLOps.NET.Extensions;
using MLOps.NET.SQLite;
using System;
using System.Diagnostics;
using System.Linq;

namespace MLOps.NET.BinaryClassification
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            var stopwatch = new Stopwatch();

            // MLOps: Create experiment and run
            var mlOpsContext = new MLOpsBuilder()
                .UseSQLite()
                .UseLocalFileModelRepository()
                .Build();

            Console.WriteLine("Creating an MLOps Run");
            var run = await mlOpsContext.LifeCycle.CreateRunAsync("Titanic Survival Predictor");
            Console.WriteLine($"Run created with Id {run.RunId}");

            var mlContext = new MLContext(seed: 1);

            Console.WriteLine("Loading the data");
            var data = mlContext.Data.LoadFromTextFile<ModelInput>("Data/titanic.csv", hasHeader: true, separatorChar: ',');
            var testTrainTest = mlContext.Data.TrainTestSplit(data);

            //MLOps: Log data - e.g. schema, columns, types and a hash to track changes
            await mlOpsContext.Data.LogDataAsync(run.RunId, data);

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
            stopwatch.Start();
            var trainer = mlContext.BinaryClassification.Trainers.SdcaLogisticRegression("Label", "Features");
            var trainingPipeline = dataProcessingPipeline
                .Append(trainer);

            var trainedModel = trainingPipeline.Fit(testTrainTest.TrainSet);

            //MLOps: Log hyperparameters for the selected trainer
            await mlOpsContext.Training.LogHyperParametersAsync(run.RunId, trainer);
            stopwatch.Stop();

            //MLOps: Training time
            await mlOpsContext.LifeCycle.SetTrainingTimeAsync(run.RunId, stopwatch.Elapsed);
            Console.WriteLine($"Training time:{mlOpsContext.LifeCycle.GetRun(run.RunId).TrainingTime}");

            Console.WriteLine("Evaluating the model");
            var predictions = trainedModel.Transform(testTrainTest.TestSet);
            var metrics = mlContext.BinaryClassification.Evaluate(predictions);

            //MLOps: Log Metrics
            Console.WriteLine("Logging metrics");
            await mlOpsContext.Evaluation.LogMetricsAsync(run.RunId, metrics);
            await mlOpsContext.Evaluation.LogConfusionMatrixAsync(run.RunId, metrics.ConfusionMatrix);

            //Save the model
            mlContext.Model.Save(trainedModel, testTrainTest.TrainSet.Schema, "BinaryClassificationModel.zip");

            //MLOps: Upload artifact/model
            Console.WriteLine("Uploading artifact");
            var runArtifact = await mlOpsContext.Model.UploadAsync(run.RunId, "BinaryClassificationModel.zip");

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
