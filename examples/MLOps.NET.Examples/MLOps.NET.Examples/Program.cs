using Microsoft.ML;
using MLOps.NET.MulticlassClassification.Entities;
using MLOps.NET.SQLite;
using System.Threading.Tasks;

namespace MLOps.NET.MulticlassClassification
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // MLOps: Create experiment and run
            var mlOpsContext = new MLOpsBuilder()
                .UseSQLite(@"C:/MLOps")
                .Build();

            var runId = await mlOpsContext.CreateRunAsync("Product Category Predictor");

            var mlContext = new MLContext(seed: 1);

            //Load data
            var data = mlContext.Data.LoadFromTextFile<ProductInformation>("Data/ecommerce.csv", hasHeader: true, separatorChar: ',');
            var testTrainTest = mlContext.Data.TrainTestSplit(data);

            //Transform
            var dataProcessingPipeline = mlContext.Transforms.Conversion.MapValueToKey(nameof(ProductInformation.Category))
                .Append(mlContext.Transforms.Text.FeaturizeText(nameof(ProductInformation.ProductName)))
                .Append(mlContext.Transforms.Text.FeaturizeText(nameof(ProductInformation.Description)))
                .Append(mlContext.Transforms.Categorical.OneHotHashEncoding(nameof(ProductInformation.Brand))
                .Append(mlContext.Transforms.Concatenate("Features", nameof(ProductInformation.ProductName), nameof(ProductInformation.Description), nameof(ProductInformation.Brand), nameof(ProductInformation.Price))
                .Append(mlContext.Transforms.NormalizeMinMax("Features"))));

            //Train the model
            var trainingPipeline = dataProcessingPipeline
                .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy(nameof(ProductInformation.Category), "Features")
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel")));

            var trainedModel = trainingPipeline.Fit(testTrainTest.TrainSet);

            //Evaluate the model
            var predictions = trainedModel.Transform(testTrainTest.TestSet);
            var metrics = mlContext.MulticlassClassification.Evaluate(predictions, nameof(ProductInformation.Category));

            //MLOps: Log Metrics
            await mlOpsContext.LogMetricsAsync(runId, metrics);

            //Save the model
            mlContext.Model.Save(trainedModel, testTrainTest.TrainSet.Schema, "MultiClassificationModel.zip");

            //MLOps: Upload artifact/model
            await mlOpsContext.UploadModelAsync(runId, "MultiClassificationModel.zip");
        }
    }
}
