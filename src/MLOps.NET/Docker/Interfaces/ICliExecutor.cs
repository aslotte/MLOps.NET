using System.Threading.Tasks;

namespace MLOps.NET.Docker.Interfaces
{
    /// <summary>
    /// Executes CLI commands
    /// </summary>
    public interface ICliExecutor
    {
        /// <summary>
        /// Installs the dotnet new template package ML.NET.Templates
        /// </summary>
        /// <returns></returns>
        Task InstallTemplatePackage();

        /// <summary>
        /// Uninstall the dotnet new template package for ML.NET.Templates
        /// </summary>
        /// <returns></returns>
        Task UninstallTemplatePackage();

        /// <summary>
        /// Creates a template project
        /// </summary>
        /// <returns></returns>
        Task CreateTemplateProject();

        /// <summary>
        /// Runs docker build
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        Task RunDockerBuild(string tagName);

        /// <summary>
        /// Runs docker push
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        Task RunDockerPush(string tagName);
    }
}
