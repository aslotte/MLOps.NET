using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MLOps.NET.Entities.Impl;

namespace MLOps.NET.Storage.EntityMaps
{
    internal sealed class DatanMap : IEntityTypeConfiguration<Data>
    {
        public void Configure(EntityTypeBuilder<Data> builder)
        {
            builder.Property(x => x.DataHash).IsRequired(false);
        }
    }
}
