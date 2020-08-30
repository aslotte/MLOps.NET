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
            context.DeploymentTargets.RemoveRange(context.DeploymentTargets);

            await context.SaveChangesAsync();
        }
    }
}
