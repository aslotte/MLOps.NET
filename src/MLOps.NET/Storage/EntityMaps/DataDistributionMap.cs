using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MLOps.NET.Entities.Impl;

namespace MLOps.NET.Storage.EntityMaps
{
    internal sealed class DataDistributionMap : IEntityTypeConfiguration<DataDistribution>
    {
        public void Configure(EntityTypeBuilder<DataDistribution> builder)
        {
            builder.Property(x => x.Value).IsRequired();
            builder.Property(x => x.Count).IsRequired();
        }
    }
}
