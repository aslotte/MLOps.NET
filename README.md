## MLOps.NET
[![Join the chat at https://gitter.im/aslotte/mlops.net](https://badges.gitter.im/aslotte/mlops.net.svg)](https://gitter.im/aslotte/mlops.net?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

![.NET Core](https://github.com/aslotte/MLOps.NET/workflows/.NET%20Core/badge.svg)

![mlops.neticon](https://img.shields.io/nuget/v/MLOps.NET.svg)

MLOps.NET is an data science tool to track and manage the lifecycle of an [ML.NET](https://github.com/dotnet/machinelearning) machine learning model.

- Experiment tracking (SQLite, SQLServer, CosmosDb)
  - Experiments
  - Runs
  - Training time
  - Evaluation metrics
  - Hyper parameters
- Data tracking
  - Data schema
  - Data quantity
  - Data hash
  - Data distribution (in progress)
- Model repostiory (Azure, AWS, local fileshare)
  - Run artifacts
  - Versioned registered models
- Model deployment
  - URI based deployment (in progress)
  - Containerized deployment (in roadmap)
  - Manual deployment (in roadmap)
  
A client application to vizualize and manage the ML lifecycle is currently in the [roadmap](https://github.com/aslotte/MLOps.NET/blob/master/images/roadmap.png) to be worked on.

### Getting started

`MLOps.NET` revolves around an `MLOpsContext`. The `MLOpsContext` contains catalogs for e.g.`Lifecycle`, `Data`, `Training`, `Evaluation` and `Deployment` to access operations helpful to manage your models lifecycle.

To create an `MLOpsContext`, use the `MLOpsBuilder` with your desired configuration. You can mix and match the location of your model repository and metadata store as you please.

#### Azure with CosmosDb
```
            IMLOpsContext mlOpsContext = new MLOpsBuilder()
                .UseCosmosDb("accountEndPoint", "accountKey")
                .UseAzureBlobModelRepository("connectionString")
                .Build();
```

#### SQL Server with Local model repository
```
            IMLOpsContext mlOpsContext = new MLOpsBuilder()
                .UseSQLServer("connectionString")
                .UseLocalFileModelRepository()
                .Build();
```

#### AWS with SQLite
```
            IMLOpsContext mlOpsContext = new MLOpsBuilder()
                .UseSQLite()
                .UseAWSS3Repository("awsAccessKey", "awsSecretAccessKey", "regionName", "bucketName")
                .Build();
```

#### Experiment tracking
To manage the lifecycle of a model, we'll need to track things such as the models evaluation metrics, hyper-parameters used during training and so forth. We organize this under the concept of experiments and runs. An experiment is the logical grouping of a model we are trying to develop, e.g. a fraud classifier or recommendation engine. For a given experiment, we can create a number of runs. Each run represent one attempt to train a given model, which is associated with the run conditions and evaluation metrics achieved. 

To create an `Experiment` and a `Run`, access the `Lifecycle` catalog on the `MLOpsContext`
```
            var experimentId = await mlOpsContext.LifeCycle.CreateExperimentAsync("FraudClassifier");

            var runId = await mlOpsContext.LifeCycle.CreateRunAsync(experimentId, "{optional Git SHA}");
```

For simplicity, you can also create an experiment (if it does not yet exist) and a run in one line
```
            var runId = await mlOpsContext.LifeCycle.CreateRunAsync("FraudClassifier", "{optional Git SHA}");
```

With an `Experiment` and a `Run` created, we can track the model training process.

##### Hyperparameters
You can access the operations necessary to track hyperparameters on the `Training` catalog. You can either track individual hyperparameters, such as number of epocs as follows:
```
            await mlOpsContext.Training.LogHyperParameterAsync(runId, "NumberOfEpochs", epocs);
```

Alternativly you can pass in the entire appended trainer and MLOps.NET will automatically log all of the trainer's hyperparameters for you
```
           await mlOpsContext.Training.LogHyperParameterAsync<SdcaLogisticRegressionBinaryTrainer>(runId, trainer);
```

##### Evaluation metrics
You can access the operations necessary to track evaluation metrics on the `Evaluation` catalog. Similarly to tracking hyperparameters, you can either log individual evaluation metrics as follows:
```
          await mlOpsContext.Evaluation.LogMetricAsync(runId, "F1Score", 0.99d);
```

Alternativly you can pass the entire `ML.NET` evaluation metric result and `MLOps.NET` will log all related evaulation metrics for you automatically.
```
          await mlOpsContext.Evaluation.LogMetricsAsync<CalibratedBinaryClassificationMetrics>(runId, metric);
```


#### Data tracking
TBD

#### Model repository
TBD

#### Model deployment
TBD 

## Contribute
We welcome contributors! Before getting started, take a moment to read our [contributing guidelines](https://github.com/aslotte/MLOps.NET/blob/master/Contributing.md)

## Code of Conduct
Please take a moment to read our [code of conduct](https://github.com/aslotte/MLOps.NET/blob/master/CODE_OF_CONDUCT.md) 

