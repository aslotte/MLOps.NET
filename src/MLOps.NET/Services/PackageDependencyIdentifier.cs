using Microsoft.Extensions.DependencyModel;
using MLOps.NET.Entities.Impl;
using MLOps.NET.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MLOps.NET.Services
{
    internal sealed class PackageDependencyIdentifier : IPackageDependencyIdentifier
    {
        public List<PackageDependency> IdentifyPackageDependencies()
        {
            return DependencyContext.Load(Assembly.GetEntryAssembly())
                .CompileLibraries
                .Where(x => x.Name.Contains("Microsoft.ML"))
                .Where(x => x.Type == "package")
                .Select(x =>
                {
                    return new PackageDependency
                    {
                        Name = x.Name,
                        Version = x.Version
                    };
                }).ToList();
        }
    }
}
