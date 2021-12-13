using SpotifyAPI.Core.Database;

namespace SpotifyAPI.Core.Modules.Authors.Entities;

public class Author : Entity
{
    public string Name { get; set; } = string.Empty;
}
