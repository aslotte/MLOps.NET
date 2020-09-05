using CommandLine;

namespace MLOps.NET.CLI
{
    [Verb("config-s3", HelpText = "Set configuration values for AWS S3")]
    internal sealed class ConfigAWSS3Options
    {
        [Option("access-key-id", Required = true)]
        public string AwsAccessKeyId { get; set; }

        [Option("secret-access-key", Required = true)]
        public string AwsSecretAccessKey { get; set; }

        [Option("region-name", Required = true)]
        public string RegionName { get; set; }
    }
}
