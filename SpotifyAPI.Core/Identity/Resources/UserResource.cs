using System;

namespace SpotifyAPI.Core.Identity.Resources;

public record UserResource
{
    public Guid Id { get; set; }
    public string? FullName { get; set; }
    public string UserName { get; set; } = null!;
    public string NormalizedUsername { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string NormalizedEmail { get; set; } = null!;
    public bool EmailConfirmed { get; set; }
}