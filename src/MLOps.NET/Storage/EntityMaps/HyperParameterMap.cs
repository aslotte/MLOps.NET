using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MLOps.NET.Entities.Impl;

namespace MLOps.NET.Storage.EntityMaps
{
    internal sealed class HyperParameterMap : IEntityTypeConfiguration<HyperParameter>
    {
        public void Configure(EntityTypeBuilder<HyperParameter> builder)
        {
            builder.Property(x => x.ParameterName).IsRequired();
            builder.Property(x => x.Value).IsRequired();
        }
    }
}
