using System;
using System.Threading.Tasks;
using FluentValidation;
using Mapster;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Opw.HttpExceptions;
using SpotifyAPI.Core.Bus;
using SpotifyAPI.Core.Database;
using SpotifyAPI.Core.Identity.Entities;
using SpotifyAPI.Core.Identity.Extensions;
using SpotifyAPI.Core.Modules.Playlist.Resources;

namespace SpotifyAPI.Core.Modules.Playlists.Commands
{
    public static class EditPlaylist
    {
        public static AuthorizationPolicy Policy { get; } = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();

        public record Request
        (
            Guid Id,
            string? Title
        ) : IAuthorizationRequirement;

        public record Response : PlaylistResource;

        public class Validator : AbstractValidator<Request>
        {
            public Validator ()
            {
                RuleFor(_ => _.Title)
                    .MinimumLength(3)
                    .When(_ => _.Title is not null)
                    .MaximumLength(200)
                    .When(_ => _.Title is not null);
            }
        }

        public class Authorization : AuthorizationHandler<Request>
        {
            private readonly AppDb _db;

            public Authorization (AppDb db)
            {
                _db = db;
            }

            protected override async Task HandleRequirementAsync (
                AuthorizationHandlerContext context,
                Request requirement
            )
            {
                var playlist = await _db.FindAsync<Entities.Playlist>(requirement.Id);

                if (playlist?.CreatedById == context.User.GetId()) context.Succeed(requirement);

                if (context.User.IsInRole(Role.User)) context.Succeed(requirement);
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
                var playlist = await _db.Set<Entities.Playlist>()
                    .Include(_ => _.CreatedBy)
                    .FirstAsync(_ => _.Id == context.Message.Id);

                if (playlist is null) throw new BadRequestException("Entity with given ID not found");

                context.Message.Adapt(playlist);

                await _db.SaveChangesAsync();

                await context.RespondIfAcceptedAsync(playlist.Adapt<Response>());
            }
        }
    }
}