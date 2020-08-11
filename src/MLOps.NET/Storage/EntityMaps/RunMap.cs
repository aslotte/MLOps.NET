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

            builder.HasMany<RunArtifact>()
                .WithOne()
                .HasForeignKey(x => x.RunId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany<HyperParameter>()
                .WithOne()
                .HasForeignKey(x => x.RunId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany<Metric>()
                .WithOne()
                .HasForeignKey(x => x.RunId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<ConfusionMatrixEntity>()
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
