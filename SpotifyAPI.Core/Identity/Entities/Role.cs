using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SpotifyAPI.Core.Identity.Entities;

public class Role : IdentityRole<Guid>
{
    public const string User = "User";

    public class Configuration : IEntityTypeConfiguration<Role>
    {
        public void Configure (EntityTypeBuilder<Role> builder)
        {
            builder.HasData(
                new Role
                {
                    Id = Guid.Parse("e4cfb454-89bb-47bc-854a-515ad7fa67f7"),
                    ConcurrencyStamp = "e0295e1b-83e8-428d-a4c9-9a0c740a60d5",
                    Name = "User",
                    NormalizedName = "USER",
                }
            );
        }
    }
}