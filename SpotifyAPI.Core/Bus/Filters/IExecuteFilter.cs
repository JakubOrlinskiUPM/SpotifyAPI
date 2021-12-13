using GreenPipes;
using MassTransit.Courier;

namespace SpotifyAPI.Core.Bus.Filters
{
    public interface IExecuteFilter<TArguments> : IFilter<ExecuteContext<TArguments>> where TArguments : class { }
}