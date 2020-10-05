## ⚡ MLOps.NET
[![Join the chat at https://gitter.im/aslotte/mlops.net](https://badges.gitter.im/aslotte/mlops.net.svg)](https://gitter.im/aslotte/mlops.net?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) ![.NET Core](https://github.com/aslotte/MLOps.NET/workflows/.NET%20Core/badge.svg) ![mlops.neticon](https://img.shields.io/nuget/v/MLOps.NET.svg)

MLOps.NET is a data science tool to track and manage the lifecycle of a [ML.NET](https://github.com/dotnet/machinelearning) machine learning model.

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
  - Data distribution
- Model repostiory (Azure Blob Storage, AWS S3, local)
  - Run artifacts
  - Versioned registered models
- Model deployment (Azure Blob Storage, AWS S3, local)
  - URI based deployment
  - Containerized deployments to a Kubernetes cluster
  - Manual deployment (in roadmap)
  
A client application to vizualize and manage the ML lifecycle is currently in the [roadmap](https://github.com/aslotte/MLOps.NET/blob/master/images/roadmap.png) to be worked on.

### Articles
- [Announcing MLOps.NET v1.2.0](https://www.alexanderslotte.com/announcing-mlops-net-v1-2/)

### Getting started

`MLOps.NET` revolves around an `MLOpsContext`. The `MLOpsContext` contains catalogs for e.g.`Lifecycle`, `Data`, `Training`, `Evaluation` and `Deployment` to access operations helpful to manage your model's lifecycle.

To create an `MLOpsContext`, use the `MLOpsBuilder` with your desired configuration. You can mix and match the location of your model repository and metadata store as you please.

#### Azure with CosmosDb
```csharp
  IMLOpsContext mlOpsContext = new MLOpsBuilder()
    .UseCosmosDb("accountEndPoint", "accountKey")
    .UseAzureBlobModelRepository("connectionString")
    .Build();
```

#### SQL Server with Local model repository
```csharp
  IMLOpsContext mlOpsContext = new MLOpsBuilder()
    .UseSQLServer("connectionString")
    .UseLocalFileModelRepository()
    .Build();
```

#### AWS with SQLite
```csharp
  IMLOpsContext mlOpsContext = new MLOpsBuilder()
    .UseSQLite()
    .UseAWSS3ModelRepository("awsAccessKey", "awsSecretAccessKey", "regionName")
    .Build();
```

#### With a Container Registry and a Kubernetes Cluster
```csharp
  IMLOpsContext mlOpsContext = new MLOpsBuilder()
    .UseLocalFileModelRepository()
    .UseSQLite()
    .UseContainerRegistry("RegistryName", "UserName", "Password")
    .UseKubernetes("kubeconfigPathOrContent")
    .Build();
```

#### Experiment tracking
To manage the lifecycle of a model, we'll need to track things such as the model's evaluation metrics, hyper-parameters used during training and so forth. We organize this under the concept of experiments and runs. An experiment is the logical grouping of a model we are trying to develop, e.g. a fraud classifier or recommendation engine. For a given experiment, we can create a number of runs. Each run represents one attempt to train a given model, which is associated with the run conditions and evaluation metrics achieved. 

To create an `Experiment` and a `Run`, access the `Lifecycle` catalog on the `MLOpsContext`
```csharp
  var experimentId = await mlOpsContext.LifeCycle.CreateExperimentAsync();

  var run = await mlOpsContext.LifeCycle.CreateRunAsync(experimentId, "{optional Git SHA}");
```

For simplicity, you can also create an experiment (if it does not yet exist) and a run in one line
```csharp
  var run = await mlOpsContext.LifeCycle.CreateRunAsync(experimentName: "FraudClassifier", "{optional Git SHA}");
```

With an `Experiment` and a `Run` created, we can track the model training process.

##### Hyperparameters
You can access the operations necessary to track hyperparameters on the `Training` catalog. You can either track individual hyperparameters, such as number of epocs as follows:
```csharp
  await mlOpsContext.Training.LogHyperParameterAsync(runId, "NumberOfEpochs", epocs);
```

Alternatively, you can pass in the entire appended trainer and MLOps.NET will automatically log all of the trainer's hyperparameters for you
```csharp
  await mlOpsContext.Training.LogHyperParameterAsync<SdcaLogisticRegressionBinaryTrainer>(runId, trainer);
```

##### Evaluation metrics
You can access the operations necessary to track evaluation metrics on the `Evaluation` catalog. Similarly to tracking hyperparameters, you can either log individual evaluation metrics as follows:
```csharp
  await mlOpsContext.Evaluation.LogMetricAsync(runId, "F1Score", 0.99d);
```

Alternatively, you can pass the entire `ML.NET` evaluation metric result and `MLOps.NET` will log all related evaluation metrics for you automatically.
```csharp
  await mlOpsContext.Evaluation.LogMetricsAsync<CalibratedBinaryClassificationMetrics>(runId, metric);
```


#### Data tracking
There are a number of useful methods on the `Data` catalog to track the data used for training. This will give you a nice audit trail to understand what data was used to train a specific model, as well as how the data looked and if it has changed in between models.

To log the data schema and the data hash (to be used to compare data for two different models), you can use the `LogDataAsync` method
```csharp
  await mlOpsContext.Data.LogDataAsync(runId, dataView);
```

To log the distribution of a given column, e.g. how many rows in a given dataset are positive and how many are negative, use the `LogDistributionAsync` method
```csharp
  await mlOpsContext.Data.LogDataDistribution<bool>(run.RunId, dataView, nameof(Review.Sentiment));
```

#### Model repository
The end product of any model development effort is the actual model itself. `MLOps.NET` offers the ability to store your model either in a storage account in Azure, an S3 bucket in AWS or locally on a fileshare of your choosing. 

To upload a model from a run
```csharp
  var runArtifact = await mlOpsContext.Model.UploadAsync(runId, "pathToModel");
```

To register a model for deployment
```csharp
  var registeredModel = await mlOpsContext.Model.RegisterModel(experimentId, runArtifact.RunArtifactId, registeredBy: "John Doe", description: "Altered weights");
```

#### Model deployment
Once a model has been registered, it's possible to deploy it to a given deployment target. A deployment target can be thought of as a specific environment from which you can serve your model, e.g. Test, Stage and Production. `MLOps.NET` currently supports deploying the model to an URI so that an ASP.NET Core application can consume it, or to a Kubernetes cluster so that the model can be consumed through a RESTful endpoint.

Methods to deploy a model can be found on the `Deployment` catalog. 
To deploy a model, start by creating a deployment target:
```csharp
var deploymentTarget = await mlOpsContext.Deployment.CreateDeploymentTargetAsync(deploymentTargetName: "Test", isProduction: false);
```

##### Deploy a model to a URI
Given a deployment target and a registered model, you can then deploy the model to a URI
```csharp
  var deployment = await mlOpsContext.Deployment.DeployModelToUriAsync(deploymentTarget, registeredModel, deployedBy: "John Doe");
```
The model is deployed to `deployment.DeploymentUri`, which can be used by a consuming application. It's also possible to get the URI/path to deployed model by doing the following:
```csharp
  var deployment = await mlOpsContext.Deployment.GetDeployments()
    .FirstOrDefault(x => x.DeploymentTarget.Name == "Test");

  var deploymentUri = await mlOpsContext.Deployment.GetDeploymentUri(deployment);
```

Deploying a model for an experiment to a given deployment target, e.g. Test, will automatically overwrite the existing model, thus the consuming application will not need to update it's URI/path to the model it's consuming. `ML.NET` will automatically poll for changes to the file making it seamless and allowing the consuming application and the ML.NET model to have different release cycles.

##### Deploy a model to Kubernetes
To deploy a model to Kubernetes you'll need to configure a Container Registry and a Kubernetes cluster via the `MLOpsBuilder`.
`MLOps.NET` is agnostic of cloud provider so you can have your container registry either live locally or in the cloud (private/public). You are free to host your Kubernetes cluster either in Azure, AWS or elsewhere, the tool simply finds it using the provided kubeconfig. Note that the `UseKubernetes` method either takes the absolute path to the  kubeconfig or the content of the kubeconfig itself, which can be useful if we are configuring it via a CI pipeline. 

```csharp
  IMLOpsContext mlOpsContext = new MLOpsBuilder()
    .UseLocalFileModelRepository()
    .UseSQLite()
    .UseContainerRegistry("RegistryName", "UserName", "Password")
    .UseKubernetes("kubeconfigPathOrContent")
    .Build();
```

We can then deploy the model to the Kubernetes cluster 
```csharp
  var deployment = await sut.Deployment.DeployModelToKubernetesAsync<ModelInput, ModelOutput>(deploymentTarget, registeredModel, "deployedBy");
  
  deployment.deploymentUri
  //e.g. http://52.146.48.228/api/Prediction
```

If you don't know the `ModelInput` and `ModelOutput` at deployment time, you can register the model schema during the run
```csharp
  await sut.LifeCycle.RegisterModelSchema<ModelInput, ModelOutput>(run.RunId);
```

This simplifies the call at deployment time
```csharp
  var deployment = await sut.Deployment.DeployModelToKubernetesAsync(deploymentTarget, registeredModel, "deployedBy");
  
  deployment.deploymentUri
  //e.g. http://52.146.48.228/api/Prediction
```

## Contribute
We welcome contributors! Before getting started, take a moment to read our [contributing guidelines](https://github.com/aslotte/MLOps.NET/blob/master/Contributing.md) as well as the [docs for new developers](https://github.com/aslotte/MLOps.NET/wiki/Contributing-to-the-repository) on how to set up your local environment.

## Code of Conduct
Please take a moment to read our [code of conduct](https://github.com/aslotte/MLOps.NET/blob/master/CODE_OF_CONDUCT.md) 

