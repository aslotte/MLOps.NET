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
                .HasMany(x => x.DataDistributions)
                .WithOne()
                .HasForeignKey(x => x.DataColumnId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
