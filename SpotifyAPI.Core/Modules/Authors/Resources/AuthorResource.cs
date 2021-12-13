using System;

namespace SpotifyAPI.Core.Modules.Authors.Resources;

public record AuthorResource
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
}