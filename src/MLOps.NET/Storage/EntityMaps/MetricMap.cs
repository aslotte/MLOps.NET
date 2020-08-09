using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MLOps.NET.Entities.Impl;

namespace MLOps.NET.Storage.EntityMaps
{
    internal sealed class MetricMap : IEntityTypeConfiguration<Metric>
    {
        public void Configure(EntityTypeBuilder<Metric> builder)
        {
            builder.Property(x => x.Value).IsRequired();
            builder.Property(x => x.MetricName).IsRequired();
        }
    }
}
