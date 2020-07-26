using MLOps.NET.Storage.Interfaces;
using System.Threading.Tasks;

namespace MLOps.NET.IntegrationTests
{
    public class RepositoryTests
    {
        protected IMLOpsContext sut;

        protected async Task TearDown(IMLOpsDbContext context)
        {
            var experiments = context.Experiments;
            var runs = context.Runs;
            var metrics = context.Metrics;
            var hyperParameters = context.HyperParameters;
            var confusionMatrices = context.ConfusionMatrices;
            var data = context.Data;
            var runArtifacts = context.RunArtifacts;
            var registeredModels = context.RegisteredModels;

            context.Experiments.RemoveRange(experiments);
            context.Runs.RemoveRange(runs);
            context.Metrics.RemoveRange(metrics);
            context.HyperParameters.RemoveRange(hyperParameters);
            context.ConfusionMatrices.RemoveRange(confusionMatrices);
            context.Data.RemoveRange(data);
            context.RunArtifacts.RemoveRange(runArtifacts);
            context.RegisteredModels.RemoveRange(registeredModels);

            await context.SaveChangesAsync();
        }
    }
}
