using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MLOps.NET.Entities.Impl;

namespace MLOps.NET.Storage.EntityMaps
{
    internal sealed class DataColumnMap : IEntityTypeConfiguration<DataColumn>
    {
        public void Configure(EntityTypeBuilder<DataColumn> builder)
        {
            builder
                .HasOne(x => x.DataDistribution)
                .WithOne()
                .HasForeignKey<DataDistribution>(x => x.DataColumnId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
