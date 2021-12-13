using System;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SpotifyAPI.Core.Database;
using SpotifyAPI.Core.Database.Pagination;
using SpotifyAPI.Core.Identity.Entities;

namespace SpotifyAPI.Core.Modules.Playlists.Queries;

public class GetPlaylistById
{
    public static AuthorizationPolicy Policy { get; } = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireRole(Role.User)
        .Build();

    public class Request : QueryModel
    {
        public Request(Guid? id) { Id = id; }

        public Guid? Id;
    }

    public class Response : Entities.Playlist { }

    public class Handler : IConsumer<Request>
    {
        private readonly AppDb _db;

        public Handler (AppDb db)
        {
            _db = db;
        }

        public Task Consume (ConsumeContext<Request> context)
        {
            var result = _db
                .Set<Entities.Playlist>()
                .AsNoTracking().First(p => p.Id == context.Message.Id);

            return context.RespondAsync(result.Adapt<Response>());
        }
    }
}