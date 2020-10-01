using System;

namespace MLOps.NET.Entities.Impl
{
    /// <summary>
    /// Entity holding information about a package dependency
    /// </summary>
    public sealed class PackageDepedency
    {
        /// <summary>
        /// PackageDepedencyId
        /// </summary>
        public Guid PackageDependencyId { get; set; }

        /// <summary>
        /// Associated RunId
        /// </summary>
        public Guid RunId { get; set; }

        /// <summary>
        /// Package Dependency Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Package Dependency Version
        /// </summary>
        public string Version { get; set; }
    }
}
