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
using SpotifyAPI.Core.Modules.Authors.Resources;

namespace SpotifyAPI.Core.Modules.Authors.Queries;

public static class GetAuthorsById
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
    public class Response : Entities.Author { }

    public class Handler : IConsumer<Request>
    {
        private readonly AppDb _db;

        public Handler (AppDb db)
        {
            _db = db;
        }

        public Task Consume (ConsumeContext<Request> context)
        {
            var result = _db.Set<Entities.Author>()
                .AsNoTracking().First(p => p.Id == context.Message.Id);

            return context.RespondAsync(result.Adapt<Response>());
        }
    }
}