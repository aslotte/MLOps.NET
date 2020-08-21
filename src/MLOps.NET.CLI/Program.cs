using CommandLine;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace MLOps.NET.CLI
{
    class Program
    {
        public static void Main(string[] args)
        {
    
         

            var commandHelper = new CommandHelper();

            Parser.Default.ParseArguments<ListRunsOptions, ListRunArtifactsOptions, ListMetricsOptions,ConfigCosmosOptions>(args)
                    .WithParsed<ListRunsOptions>(commandHelper.ListRuns)
                    .WithParsed<ListRunArtifactsOptions>(commandHelper.ListRunArtifacts)
                    .WithParsed<ListMetricsOptions>(commandHelper.ListMetrics)
                    .WithParsed<ConfigCosmosOptions>(commandHelper.SetCosmosConfiguration);
        }

    }
}
