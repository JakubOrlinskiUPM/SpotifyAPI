using GreenPipes;
using MassTransit;

namespace SpotifyAPI.Core.Bus.Filters
{
    public interface ISendFilter<T> : IFilter<SendContext<T>> where T : class { }

    public interface ISendFilter : IFilter<SendContext> { }
}