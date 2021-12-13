using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;
using Microsoft.AspNetCore.Http;
using SpotifyAPI.Core.Bus.Filters;
using SpotifyAPI.Core.Identity.Extensions;

namespace SpotifyAPI.Core.Identity.Filters
{
    public class UserProviderFilter : IPublishFilter, ISendFilter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserProviderFilter (IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task Send (PublishContext context, IPipe<PublishContext> next)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext?.User.GetId() is not null) context.SetUserId(httpContext.User.GetId()!.Value);

            await next.Send(context);
        }

        public void Probe (ProbeContext context) { }

        public async Task Send (SendContext context, IPipe<SendContext> next)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext?.User.GetId() is not null) context.SetUserId(httpContext.User.GetId()!.Value);

            await next.Send(context);
        }
    }

    public static class UserProviderFilterExtensions
    {
        public const string UserIdHeaderKey = "UserId";

        public static Guid? GetUserId (this IEnumerable<HeaderValue> headers)
        {
            if (Guid.TryParse(
                headers.FirstOrDefault(header => header.Key == UserIdHeaderKey).Value.ToString(),
                out var userId
            )) return userId;

            return null;
        }

        public static Guid? GetUserId (this MessageContext context)
        {
            return context.Headers.GetUserId();
        }

        public static T SetUserId<T> (this T context, Guid userId) where T : SendContext
        {
            context.Headers.Set(UserIdHeaderKey, userId);

            return context;
        }
    }
}