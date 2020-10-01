using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MLOps.NET.Entities.Impl;

namespace MLOps.NET.Storage.EntityMaps
{
    internal sealed class PackageDependencyMap : IEntityTypeConfiguration<PackageDepedency>
    {
        public void Configure(EntityTypeBuilder<PackageDepedency> builder)
        {
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Version).IsRequired();
        }
    }
}
