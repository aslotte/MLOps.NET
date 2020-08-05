using MLOps.NET.Storage.Interfaces;
using System.Threading.Tasks;

namespace MLOps.NET.IntegrationTests
{
    public class RepositoryTests
    {
        protected IMLOpsContext sut;

        protected async Task TearDown(IMLOpsDbContext context)
        {
            context.Experiments.RemoveRange(context.Experiments);
            context.Runs.RemoveRange(context.Runs);
            context.Metrics.RemoveRange(context.Metrics);
            context.HyperParameters.RemoveRange(context.HyperParameters);
            context.ConfusionMatrices.RemoveRange(context.ConfusionMatrices);
            context.Data.RemoveRange(context.Data);
            context.RunArtifacts.RemoveRange(context.RunArtifacts);
            context.RegisteredModels.RemoveRange(context.RegisteredModels);
            context.DeploymentTargets.RemoveRange(context.DeploymentTargets);

            await context.SaveChangesAsync();
        }
    }
}
