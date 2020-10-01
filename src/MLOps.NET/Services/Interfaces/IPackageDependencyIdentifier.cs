using System.Collections.Generic;

namespace MLOps.NET.Services.Interfaces
{
    /// <summary>
    /// Identifies ML.NET Package Depedencies
    /// </summary>
    public interface IPackageDependencyIdentifier
    {
        /// <summary>
        /// Identify ML.NET Package Dependencies
        /// </summary>
        /// <returns></returns>
        List<(string Name, string Version)> IdentifyPackageDependencies();
    }
}
