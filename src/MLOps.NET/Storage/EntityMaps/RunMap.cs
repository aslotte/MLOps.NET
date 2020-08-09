using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MLOps.NET.Entities.Impl;

namespace MLOps.NET.Storage.EntityMaps
{
    internal sealed class RunMap : IEntityTypeConfiguration<Run>
    {
        public void Configure(EntityTypeBuilder<Run> builder)
        {
            builder.Property(x => x.GitCommitHash).IsRequired(false);
            builder.Property(x => x.RunDate).IsRequired();
            builder.Property(x => x.TrainingTime).IsRequired(false);

            builder.HasMany(x => x.RunArtifacts)
                .WithOne(y => y.Run)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.HyperParameters)
                .WithOne(y => y.Run)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Metrics)
                .WithOne(y => y.Run)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
