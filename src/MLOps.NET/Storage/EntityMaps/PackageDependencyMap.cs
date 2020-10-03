using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MLOps.NET.Entities.Impl;

namespace MLOps.NET.Storage.EntityMaps
{
    internal sealed class PackageDependencyMap : IEntityTypeConfiguration<PackageDependency>
    {
        public void Configure(EntityTypeBuilder<PackageDependency> builder)
        {
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Version).IsRequired();
        }
    }
}
