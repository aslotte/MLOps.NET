using System.IO;

namespace MLOps.NET.Docker.Settings
{
    /// <summary>
    /// DockerSettings
    /// </summary>
    public sealed class DockerSettings
    {
        /// <summary>
        /// Name of Docker Registry
        /// </summary>
        public string RegistryName { get; set; }
        /// <summary>
        /// Username used for docker login
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Password used for docker login
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Requires authentication
        /// </summary>
        public bool RequiresAuthentication => !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password);
        /// <summary>
        /// Name of directory to build image in
        /// </summary>
        public static string DirectoryName => "image";
        /// <summary>
        /// Name of ML.NET model
        /// </summary>
        public static string ModelName => "model.zip";
        /// <summary>
        /// Verision of ML.NET.Templates
        /// </summary>
        public static string TemplatePackageVersion => "0.3.0-beta";
        /// <summary>
        /// dotnet new template name
        /// </summary>
        public static string TemplateName => "mlnet-web-embedded";

        /// <summary>
        /// Project name
        /// </summary>
        public static string ProjectName => "ML.NET.Web.Embedded.csproj";

        /// <summary>
        /// Path to template project
        /// </summary>
        public static string ProjectPath => Path.Join(Directory.GetCurrentDirectory(), DirectoryName, ProjectName);
    }
}
