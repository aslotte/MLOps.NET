using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MLOps.NET.Entities.Impl;

namespace MLOps.NET.Storage.EntityMaps
{
    internal sealed class DeploymentMap : IEntityTypeConfiguration<Deployment>
    {
        public void Configure(EntityTypeBuilder<Deployment> builder)
        {
            builder
                .HasOne(x => x.RegisteredModel)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.DeploymentTarget)
                .WithMany()
                .HasForeignKey(x => x.DeploymentTargetId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.Experiment)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
