using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MLOps.NET.Entities.Impl;

namespace MLOps.NET.Storage.EntityMaps
{
    internal sealed class ConfusionMatrixEntityMap : IEntityTypeConfiguration<ConfusionMatrixEntity>
    {
        public void Configure(EntityTypeBuilder<ConfusionMatrixEntity> builder)
        {
            builder.Property(x => x.SerializedMatrix).IsRequired();

            builder.HasKey(x => x.ConfusionMatrixEntityId);
        }
    }
}
