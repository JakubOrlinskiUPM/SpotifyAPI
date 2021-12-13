using System;
using SpotifyAPI.Core.Identity.Resources;

namespace SpotifyAPI.Core.Modules.Playlist.Resources;

public record PlaylistResource
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public UserResource? CreatedBy { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
}