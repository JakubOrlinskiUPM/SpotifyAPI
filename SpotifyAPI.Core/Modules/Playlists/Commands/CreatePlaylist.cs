using System;
using System.Threading.Tasks;
using FluentValidation;
using Mapster;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using SpotifyAPI.Core.Bus;
using SpotifyAPI.Core.Database;
using SpotifyAPI.Core.Identity.Entities;
using SpotifyAPI.Core.Identity.Filters;

namespace SpotifyAPI.Core.Modules.Playlists.Commands
{
    public static class CreatePlaylist
    {
        public static AuthorizationPolicy Policy { get; } = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireRole(Role.User)
            .Build();

        public record Request
        (
            string Title,
            string Description
        );

        public record Response(Guid Id);

        public class Validator : AbstractValidator<Request>
        {
            public Validator ()
            {
                RuleFor(_ => _.Title)
                    .NotEmpty()
                    .MinimumLength(3)
                    .MaximumLength(200);

            }
        }

        public class Handler : IConsumer<Request>
        {
            private readonly AppDb _db;

            public Handler (AppDb db)
            {
                _db = db;
            }

            public async Task Consume (ConsumeContext<Request> context)
            {
                var playlist = context.Message.Adapt<Entities.Playlist>();
                playlist.CreatedById = context.GetUserId();

                _db.Set<Entities.Playlist>().Add(playlist);
                await _db.SaveChangesAsync();

                await context.RespondIfAcceptedAsync(new Response(playlist.Id));
            }
        }
    }
}