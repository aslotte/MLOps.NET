using CliWrap;
using CliWrap.Buffered;
using MLOps.NET.Docker.Interfaces;
using MLOps.NET.Docker.Settings;
using MLOps.NET.Exceptions;
using System;
using System.Threading.Tasks;

namespace MLOps.NET.Docker
{
    ///<inheritdoc cref="ICliExecutor"/>
    internal class CliExecutor : ICliExecutor
    {
        private readonly DockerSettings dockerSettings;

        public CliExecutor(DockerSettings dockerSettings)
        {
            this.dockerSettings = dockerSettings;
        }

        ///<inheritdoc cref="ICliExecutor"/>
        public async Task InstallTemplatePackage()
        {
            try
            {
                await Cli.Wrap("dotnet")
                    .WithArguments($"new --install ML.NET.Templates::{dockerSettings.TemplatePackageVersion}")
                    .ExecuteBufferedAsync();
            }
            catch (Exception ex)
            {
                throw new TemplateInstallationException($"Unable to install dotnet new template package ML.NET.Templates with version {dockerSettings.TemplatePackageVersion}", ex);
            }
        }

        ///<inheritdoc cref="ICliExecutor"/>
        public async Task UninstallTemplatePackage()
        {
            await Cli.Wrap("dotnet")
                .WithArguments($"new -u ML.NET.Templates")
                .ExecuteBufferedAsync();
        }

        ///<inheritdoc cref="ICliExecutor"/>
        public async Task CreateTemplateProject()
        {
            try
            {
                await Cli.Wrap("dotnet")
                    .WithArguments($"new {dockerSettings.TemplateName} --force --output {dockerSettings.DirectoryName}")
                    .ExecuteBufferedAsync();
            }
            catch (Exception ex)
            {
                throw new TemplateCreationException($"Unable to create project from dotnet new template {dockerSettings.TemplateName}", ex);
            }
        }

        public async Task RunDockerBuild(string tagName)
        {
            try
            {
                await Cli.Wrap("docker")
                    .WithArguments($"build --tag {tagName.ToLower()} --file {dockerSettings.DirectoryName}/Dockerfile {dockerSettings.DirectoryName}")
                    .ExecuteBufferedAsync();
            }
            catch (Exception ex)
            {
                throw new DockerBuildException($"Unable to run docker build for {tagName}", ex);
            }
        }

        public async Task RunDockerLogin()
        {
            try
            {
                if (dockerSettings.RequiresAuthentication)
                {
                    await Cli.Wrap("docker")
                        .WithArguments($"login {dockerSettings.RegistryName} --username {dockerSettings.Username} --password {dockerSettings.Password}")
                        .ExecuteBufferedAsync();
                }
            }
            catch (Exception ex)
            {
                throw new DockerLoginException($"Unable to run docker login", ex);
            }
        }

        public async Task RunDockerPush(string tagName)
        {
            try
            {
                await Cli.Wrap("docker")
                    .WithArguments($"push {tagName.ToLower()}")
                    .ExecuteBufferedAsync();
            }
            catch (Exception ex)
            {
                throw new DockerPushException($"Unable to run docker push for {tagName}", ex);
            }
        }

        public async Task<bool> RunDockerPull(string tagName)
        {
            try
            {
               var command = await Cli.Wrap("docker")
                    .WithArguments($"pull {tagName.ToLower()}")
                    .ExecuteBufferedAsync();

                return command.ExitCode == 0;
            }
            catch (Exception ex)
            {
                throw new DockerPullException($"Unable to run docker pull for {tagName}", ex);
            }
        }

        public async Task RemoveDockerImage(string tagName)
        {
            try
            {
                await Cli.Wrap("docker")
                    .WithArguments($"image rm {tagName.ToLower()}")
                    .ExecuteBufferedAsync();
            }
            catch 
            {
                //intentionally left empty
            }
        }
    }
}
