using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Mediator;

namespace SpotifyAPI.Core.Bus;

public static class BusExtensions
{
    public static async Task<Response<TResponse>> Request<TRequest, TResponse> (
        this IMediator mediator,
        TRequest request,
        CancellationToken cancellationToken = default,
        RequestTimeout requestTimeout = default
    )
        where TRequest : class
        where TResponse : class
    {
        return await mediator.CreateRequest(request, cancellationToken, requestTimeout).GetResponse<TResponse>();
    }
    
    public static async Task RespondIfAcceptedAsync<TMessage> (this ConsumeContext context, TMessage message)
        where TMessage : class
    {
        if (context.RequestId == null) return;

        try
        {
            if (!context.IsResponseAccepted<TMessage>()) return;
        }
        catch
        {
            // Do nothing
        }

        await context.RespondAsync(message);
    }
}