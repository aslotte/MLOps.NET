using CliWrap;
using CliWrap.Buffered;
using MLOps.NET.Docker.Interfaces;
using MLOps.NET.Docker.Settings;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Exceptions;
using MLOps.NET.Kubernetes.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MLOps.NET.Docker
{
    ///<inheritdoc cref="ICliExecutor"/>
    internal class CliExecutor : ICliExecutor
    { 
        ///<inheritdoc cref="ICliExecutor"/>
        public async Task InstallTemplatePackage(DockerSettings dockerSettings)
        {
            try
            {
                Console.WriteLine("Installing dotnet new templates...");

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
        public async Task AddPackageDependencies(DockerSettings dockerSettings, List<PackageDependency> packageDependencies)
        {
            var projectPath = Path.Join(dockerSettings.DirectoryName, dockerSettings.ProjectName);

            foreach (var package in packageDependencies)
            {
                try
                {
                    Console.WriteLine($"Installing dotnet package dependency {package.Name} with version {package.Version}...");

                    await Cli.Wrap("dotnet")
                        .WithArguments($"add {projectPath} package {package.Name} --version {package.Version}")
                        .ExecuteBufferedAsync();
                }
                catch (Exception ex)
                {
                    throw new AddPackageDependencyException($"Unable to add package dependency {package.Name} with version {package.Version}", ex);
                }
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
        public async Task CreateTemplateProject(DockerSettings dockerSettings)
        {
            try
            {
                Console.WriteLine("Creating template project...");

                await Cli.Wrap("dotnet")
                    .WithArguments($"new {dockerSettings.TemplateName} --force --output {dockerSettings.DirectoryName}")
                    .ExecuteBufferedAsync();
            }
            catch (Exception ex)
            {
                throw new TemplateCreationException($"Unable to create project from dotnet new template {dockerSettings.TemplateName}", ex);
            }
        }

        public async Task RunDockerBuild(DockerSettings dockerSettings, string imageName)
        {
            try
            {
                Console.WriteLine("Running docker build...");

                await Cli.Wrap("docker")
                    .WithArguments($"build --tag {imageName.ToLower()} --file {dockerSettings.DirectoryName}/Dockerfile {dockerSettings.DirectoryName}")
                    .ExecuteBufferedAsync();
            }
            catch (Exception ex)
            {
                throw new DockerBuildException($"Unable to run docker build for {imageName}", ex);
            }
        }

        public async Task RunDockerLogin(DockerSettings dockerSettings)
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

        public async Task RunDockerPush(string imageName)
        {
            try
            {
                Console.WriteLine("Running docker push...");

                await Cli.Wrap("docker")
                    .WithArguments($"push {imageName.ToLower()}")
                    .ExecuteBufferedAsync();
            }
            catch (Exception ex)
            {
                throw new DockerPushException($"Unable to run docker push for {imageName}", ex);
            }
        }

        public async Task<bool> RunDockerPull(string imageName)
        {
            try
            {
               var command = await Cli.Wrap("docker")
                    .WithArguments($"pull {imageName.ToLower()}")
                    .ExecuteBufferedAsync();

                return command.ExitCode == 0;
            }
            catch (Exception ex)
            {
                throw new DockerPullException($"Unable to run docker pull for {imageName}", ex);
            }
        }

        public async Task CreateNamespaceAsync(KubernetesSettings kubernetesSettings, string namespaceName)
        {
            try
            {
                Console.WriteLine("Running kubectl create namespace...");

                var command = await Cli.Wrap("kubectl")
                     .WithArguments($"create namespace {namespaceName.ToLower()} --kubeconfig {kubernetesSettings.KubeConfigPath}")
                     .ExecuteBufferedAsync();
            }
            catch
            {
                //intentionally left empty
            }
        }

        public async Task CreateImagePullSecret(KubernetesSettings kubernetesSettings, DockerSettings dockerSettings, string namespaceName)
        {
            try
            {
                Console.WriteLine("Running kubectl create imagePullSecret...");

                var command = await Cli.Wrap("kubectl")
                     .WithArguments($"create secret docker-registry {kubernetesSettings.ImagePullSecretName} --namespace {namespaceName.ToLower()} --docker-server {dockerSettings.RegistryName} --docker-username {dockerSettings.Username} --docker-password {dockerSettings.Password} --kubeconfig {kubernetesSettings.KubeConfigPath}")
                     .ExecuteBufferedAsync();
            }
            catch
            {
                //intentionally left empty
            }
        }

        public async Task KubctlApplyAsync(KubernetesSettings kubernetesSettings, string manifestName)
        {
            try
            {
                Console.WriteLine($"Running kubectl apply {manifestName}...");

                var command = await Cli.Wrap("kubectl")
                     .WithArguments($"apply -f {manifestName} --kubeconfig {kubernetesSettings.KubeConfigPath}")
                     .ExecuteBufferedAsync();
            }
            catch (Exception ex)
            {
                throw new KubectlApplyException($"Unable to apply Kubernetes manifest {manifestName}", ex);
            }
        }

        public async Task RemoveDockerImage(string imageName)
        {
            try
            {
                await Cli.Wrap("docker")
                    .WithArguments($"image rm {imageName}")
                    .ExecuteBufferedAsync();
            }
            catch 
            {
                //intentionally left empty
            }
        }

        public async Task<string> GetServiceExternalIPAsync(KubernetesSettings kubernetesSettings, string experimentName, string namespaceName)
        {
            try
            {
                Console.WriteLine($"Fetching external IP...");

                int timeout = 3 * 60 * 1000;
                int timePassed = 0;
                int interval = 5 * 1000;

                while (timePassed < timeout)
                {
                    Thread.Sleep(interval);
                    timePassed += interval;

                    var command = await Cli.Wrap("kubectl")
                         .WithArguments($"get service -name {experimentName.ToLower()} --namespace {namespaceName.ToLower()} --kubeconfig {kubernetesSettings.KubeConfigPath}")
                         .ExecuteBufferedAsync();

                    using StringReader reader = new StringReader(command.StandardOutput);

                    //Skip first line with headers
                    reader.ReadLine();

                    var externalIP = reader.ReadLine().Split("   ")[3].Trim();

                    if (!string.IsNullOrEmpty(externalIP) && !externalIP.ToLower().Contains("pending"))
                    {
                        return externalIP;
                    }

                    Console.WriteLine("The external IP address is still pending...");
                }
                return "unknown";
            }
            catch
            {
                return "unknown";
            }
        }
    }
}
