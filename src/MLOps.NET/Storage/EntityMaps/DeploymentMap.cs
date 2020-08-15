using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MLOps.NET.Entities.Impl;

namespace MLOps.NET.Storage.EntityMaps
{
    internal sealed class DeploymentMap : IEntityTypeConfiguration<Deployment>
    {
        public void Configure(EntityTypeBuilder<Deployment> builder)
        {
            builder.Property(x => x.DeploymentDate).IsRequired();
            builder.Property(x => x.DeployedBy).IsRequired();
            builder.Property(x => x.DeploymentUri).IsRequired();
        }
    }
}
