using GreenPipes;
using MassTransit;

namespace SpotifyAPI.Core.Bus.Filters
{
    public interface IPublishFilter<T> : IFilter<PublishContext<T>> where T : class { }

    public interface IPublishFilter : IFilter<PublishContext> { }
}