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

public static class GetAuthors
{
    public static AuthorizationPolicy Policy { get; } = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireRole(Role.User)
        .Build();

    public class Request : QueryModel { }

    public class Response : Paginated<AuthorResource> { }

    public class Handler : IConsumer<Request>
    {
        private readonly AppDb _db;

        public Handler (AppDb db)
        {
            _db = db;
        }

        public async Task Consume (ConsumeContext<Request> context)
        {
            var result = await _db.Set<Entities.Author>()
                .AsNoTracking()
                .PaginateAs<Entities.Author, AuthorResource>(context.Message);

            await context.RespondAsync(result.Adapt<Response>());
        }
    }
}