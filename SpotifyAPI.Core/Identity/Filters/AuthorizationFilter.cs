using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Opw.HttpExceptions;
using SpotifyAPI.Core.Bus.Filters;
using SpotifyAPI.Core.Identity.Entities;

namespace SpotifyAPI.Core.Identity.Filters
{
    /// <summary>
    ///     Authorizes requests based on action authorization handlers
    /// </summary>
    /// <typeparam name="T">Request type</typeparam>
    public class AuthorizationFilter<T> : IConsumeFilter<T> where T : class
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserClaimsPrincipalFactory<User> _claimsPrincipalFactory;
        private readonly UserManager<User> _userManager;

        public AuthorizationFilter (
            IAuthorizationService authorizationService,
            UserManager<User> userManager,
            IUserClaimsPrincipalFactory<User> claimsPrincipalFactory
        )
        {
            _authorizationService = authorizationService;
            _userManager = userManager;
            _claimsPrincipalFactory = claimsPrincipalFactory;
        }

        public async Task Send (ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            var request = context.Message;
            var requirements = new List<IAuthorizationRequirement>();

            if (request is null)
            {
                await next.Send(context);
                return;
            }

            if (request.GetType().IsNested &&
                request.GetType().DeclaringType?.GetProperty("Policy")?.GetValue(request.GetType().DeclaringType) is
                    AuthorizationPolicy policy)
                requirements.AddRange(policy.Requirements);

            if (request.GetType().IsAssignableTo(typeof(IAuthorizationRequirement)))
                requirements.Add((IAuthorizationRequirement)request);

            if (!requirements.Any())
            {
                await next.Send(context);
                return;
            }

            var claimsPrincipal = new ClaimsPrincipal();

            if (context.GetUserId() is not null)
            {
                var user = await _userManager.FindByIdAsync(context.GetUserId().ToString());
                claimsPrincipal = await _claimsPrincipalFactory.CreateAsync(user);
            }

            var result = await _authorizationService.AuthorizeAsync(claimsPrincipal, request, requirements);

            if (!result.Succeeded) throw new ForbiddenException();

            await next.Send(context);
        }

        public void Probe (ProbeContext context) { }
    }
}