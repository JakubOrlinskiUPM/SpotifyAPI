using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpotifyAPI.Core.Database;

namespace SpotifyAPI.Core.Modules.Playlists.Entities;

public class Playlist : Entity
{
    public string Title { get; set; } = string.Empty;
    public Guid? CreatedById { get; set; }

    public User? CreatedBy { get; set; }

    public class Configuration : EntityConfiguration<Playlist>
    {
        public override void Configure (EntityTypeBuilder<Playlist> builder)
        {
            base.Configure(builder);
            builder.HasOne(_ => _.CreatedBy).WithMany(_ => _.CreatedPlaylists);
        }
    }
}

public partial class User
{
    public List<Playlist> CreatedPlaylists { get; set; } = new();
}