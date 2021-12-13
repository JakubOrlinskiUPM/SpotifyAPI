using GreenPipes;
using MassTransit.Courier;

namespace SpotifyAPI.Core.Bus.Filters
{
    public interface ICompensateFilter<TLog> : IFilter<CompensateContext<TLog>> where TLog : class { }
}