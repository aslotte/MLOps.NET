namespace MLOps.NET.CLI.Commands.Interfaces
{
    internal interface ILifeCycleCatalogCli
    {
        void CreateRun(CreateRunOptions options);

        void CreateExperiment(CreateExperimentOptions options);

        void ListRuns(ListRunsOptions options);

        void ListRunArtifacts(ListRunArtifactsOptions options);

        void ListMetrics(ListMetricsOptions options);
    }
}
