using Microsoft.Extensions.Configuration;

namespace MLOps.NET.Benchmarks
{
    public static class ConfigurationFactory
    {
        public static IConfiguration GetConfiguration(string configuration)
        {
            return new ConfigurationBuilder()
                .AddJsonFile(configuration)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
