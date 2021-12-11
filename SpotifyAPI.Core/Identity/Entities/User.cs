using Microsoft.AspNetCore.Identity;

namespace SpotifyAPI.Core.Identity.Entities;

public partial class User : IdentityUser<Guid>
{
    public string? FullName { get; set; }
}