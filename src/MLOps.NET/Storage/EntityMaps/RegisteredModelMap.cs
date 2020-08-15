using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MLOps.NET.Entities.Impl;

namespace MLOps.NET.Storage.EntityMaps
{
    internal sealed class RegisteredModelMap : IEntityTypeConfiguration<RegisteredModel>
    {
        public void Configure(EntityTypeBuilder<RegisteredModel> builder)
        {
            builder.Property(x => x.RegisteredDate).IsRequired();
            builder.Property(x => x.RegisteredBy).IsRequired();
            builder.Property(x => x.Version).IsRequired();

            builder.HasMany(x => x.Deployments)
                .WithOne()
                .HasForeignKey(x => x.RegisteredModelId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
