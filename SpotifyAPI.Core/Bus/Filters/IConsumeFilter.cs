using GreenPipes;
using MassTransit;

namespace SpotifyAPI.Core.Bus.Filters
{
    public interface IConsumeFilter<T> : IFilter<ConsumeContext<T>> where T : class { }

    public interface IConsumeFilter : IFilter<ConsumeContext> { }
}