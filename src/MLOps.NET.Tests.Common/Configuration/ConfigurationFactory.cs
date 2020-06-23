using Microsoft.Extensions.Configuration;

namespace MLOps.NET.Tests.Common.Configuration
{
    public static class ConfigurationFactory
    {
        public static IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
