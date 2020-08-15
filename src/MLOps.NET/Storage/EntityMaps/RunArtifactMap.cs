using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MLOps.NET.Entities.Impl;

namespace MLOps.NET.Storage.EntityMaps
{
    internal sealed class RunArtifactMap : IEntityTypeConfiguration<RunArtifact>
    {
        public void Configure(EntityTypeBuilder<RunArtifact> builder)
        {
            builder.Property(x => x.Name).IsRequired();

            builder.HasOne(x => x.RegisteredModel)
                .WithOne()
                .HasForeignKey<RunArtifact>(x => x.RunArtifactId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
