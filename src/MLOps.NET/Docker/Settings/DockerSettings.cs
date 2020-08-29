namespace MLOps.NET.Docker.Settings
{
    internal sealed class DockerSettings
    {
        public string RegistryName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string DirectoryName => "image";
        public string ModelName => "model.zip";
        public string TemplatePackageVersion => "0.3.0-beta";
        public string TemplateName => "mlnet-web-embedded";
    }
}
