using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MLOps.NET.Entities.Impl;

namespace MLOps.NET.Storage.EntityMaps
{
    internal sealed class ModelSchemaMap : IEntityTypeConfiguration<ModelSchema>
    {
        public void Configure(EntityTypeBuilder<ModelSchema> builder)
        {
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Definition).IsRequired();
        }
    }
}
