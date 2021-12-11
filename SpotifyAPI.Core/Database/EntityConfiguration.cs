using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace SpotifyAPI.Core.Database;

public abstract class EntityConfiguration<T> : IEntityTypeConfiguration<T> where T : Entity
{
    public virtual void Configure (EntityTypeBuilder<T> builder)
    {
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasValueGenerator<GuidValueGenerator>();
    }
}