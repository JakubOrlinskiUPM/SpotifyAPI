using MassTransit;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using SpotifyAPI.Core.Modules.Playlist.Resources;

namespace SpotifyAPI.Core.Modules.Playlist.Queries;

public static class GetPlaylists
{
    public static AuthorizationPolicy Policy { get; } = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireRole(Role.Agent)
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