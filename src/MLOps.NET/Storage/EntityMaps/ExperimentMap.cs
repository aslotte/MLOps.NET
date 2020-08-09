using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MLOps.NET.Entities.Impl;

namespace MLOps.NET.Storage.EntityMaps
{
    internal sealed class ExperimentMap : IEntityTypeConfiguration<Experiment>
    {
        public void Configure(EntityTypeBuilder<Experiment> builder)
        {
            builder.Property(x => x.ExperimentName).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();

            builder
                .HasMany(x => x.Runs)
                .WithOne(y => y.Experiment)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
