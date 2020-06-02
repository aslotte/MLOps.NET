![.NET Core](https://github.com/aslotte/MLOps.NET/workflows/.NET%20Core/badge.svg)

### What is MLOps.NET?
If you're used to creating machine learning models in libraries such as Tensorflow, Keras, Scikitlearn or H2O you may have come across libraries such as MLflow or Neptune to manage and track the life-cycle of your machine learning models. 

Models created in ML.NET can however currently not be used in MLflow, and as such the idea of MLOps.NET was spawn.

### Roadmap
The project is intended to support:
- Track model metrics and artifacts from experiments and runs
- Ability to store metrics and models both on-premise and in the cloud
- Visualize experiments, run and metrics in a Web UI (hosted on a Docker container?)
- Make models accesible for various ML.NET deployment scenarios
- Track information about the data used during training

### How can I use it?

```
var lifecycleManager = new ModelLifeCycleManager().UseAzure(connectionString);
var experimentId = await mlLifeCycleManager.CreateExperimentAsync("ModelName");
var runId = await mlLifeCycleManager.CreateRunAsync(experimentId);

...
await mlLifeCycleManager.LogMetricAsync(runId,
                nameof(metrics.Accuracy),
                metrics.Accuracy);
```
