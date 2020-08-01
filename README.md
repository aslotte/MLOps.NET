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

### AWS with SQLite
```
            IMLOpsContext mlOpsContext = new MLOpsBuilder()
                .UseSQLite()
                .UseAWSS3Repository("awsAccessKey", "awsSecretAccessKey", "regionName", "bucketName);
                .Build();
```

#### Experiment tracking
TBD

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

