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

            builder
                .HasOne(x => x.Run)
                .WithOne()
                .OnDelete(DeleteBehavior.Restrict);

            builder
                 .HasOne(x => x.Experiment)
                 .WithMany()
                 .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
