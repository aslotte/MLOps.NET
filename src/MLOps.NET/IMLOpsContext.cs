using MLOps.NET.Catalogs;

namespace MLOps.NET
{
    /// <summary>
    /// Access point to manage the lifecycle of a machine learning model
    /// 
    /// Please note that we use CosmosDb as one provider which currently have EF Core limitations
    /// https://docs.microsoft.com/en-us/ef/core/providers/cosmos/limitations
    /// </summary>
    public interface IMLOpsContext
    {
        /// <summary>
        /// Operations related to the lifecycle of a model
        /// </summary>
        LifeCycleCatalog LifeCycle { get; }

        /// <summary>
        /// Operations related to tracking the data used to train a model
        /// </summary>
        DataCatalog Data { get;  }

        /// <summary>
        /// Operations related to tracking the evaluation metrics of a model
        /// </summary>
        EvaluationCatalog Evaluation { get; }

        /// <summary>
        /// Operations related to a model
        /// </summary>
        ModelCatalog Model { get; }

        /// <summary>
        /// Operations related to the training of a model
        /// </summary>
        TrainingCatalog Training { get; }

        /// <summary>
        /// Operations related to the deployment of a model
        /// </summary>
        DeploymentCatalog Deployment { get; }
    }
}
