using System.Linq;
using System.Threading.Tasks;
using Mapster;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SpotifyAPI.Core.Database;
using SpotifyAPI.Core.Database.Pagination;
using SpotifyAPI.Core.Identity.Entities;
using SpotifyAPI.Core.Modules.Playlist.Resources;

namespace SpotifyAPI.Core.Modules.Playlists.Queries;

public static class GetPlaylists
{
    public static AuthorizationPolicy Policy { get; } = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireRole(Role.User)
        .Build();

    public class Request : QueryModel { }

    public class Response : Paginated<PlaylistResource> { }

    public class Handler : IConsumer<Request>
    {
        private readonly AppDb _db;

        public Handler (AppDb db)
        {
            _db = db;
        }

        public async Task Consume (ConsumeContext<Request> context)
        {
            var result = await _db.Set<Entities.Playlist>()
                .AsNoTracking()
                .PaginateAs<Entities.Playlist, PlaylistResource>(context.Message);

            await context.RespondAsync(result.Adapt<Response>());
        }
    }
}