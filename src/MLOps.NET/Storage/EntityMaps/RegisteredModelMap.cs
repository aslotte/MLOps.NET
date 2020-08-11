using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MLOps.NET.Entities.Impl;

namespace MLOps.NET.Storage.EntityMaps
{
    internal sealed class RegisteredModelMap : IEntityTypeConfiguration<RegisteredModel>
    {
        public void Configure(EntityTypeBuilder<RegisteredModel> builder)
        {
            builder.Property(X => X.RegisteredDate).IsRequired();
            builder.Property(X => X.RegisteredBy).IsRequired();
            builder.Property(X => X.Version).IsRequired();

            builder
                .HasOne<Run>()
                .WithOne()
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne<RunArtifact>()
                .WithOne()
                .OnDelete(DeleteBehavior.Restrict);

            builder
                 .HasOne<Experiment>()
                 .WithMany()
                 .HasForeignKey(x => x.ExperimentId)
                 .OnDelete(DeleteBehavior.Restrict);

            builder
                 .HasMany<Deployment>()
                 .WithOne()
                 .HasForeignKey(x => x.DeploymentId)
                 .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
