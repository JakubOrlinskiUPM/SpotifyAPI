using Autofac;
using GreenPipes;
using MassTransit;
using MassTransit.Definition;
using Microsoft.Extensions.Configuration;

namespace SpotifyAPI.Core.Bus
{
    public class BusModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            var assemblies = ThisAssembly;
            
            builder.RegisterConsumers(assemblies);
            builder.RegisterSagas(assemblies);
            builder.RegisterSagaStateMachines(assemblies);
            
            builder.AddMediator(
                cfg =>
                {
                    cfg.SetEndpointNameFormatter(new SnakeCaseEndpointNameFormatter(true));
                    cfg.AddSagaStateMachines(assemblies);
                    cfg.AddSagas(assemblies);
                    cfg.AddConsumers(assemblies);
                    cfg.AddActivities(assemblies);
                    builder.RegisterBuildCallback(
                        scope =>
                        {
                            var config = scope.Resolve<IConfiguration>();
                            cfg.SetRedisSagaRepositoryProvider(
                                redis => { redis.DatabaseConfiguration(config.GetConnectionString("Redis")); }
                            );
                        }
                    );
                    builder.RegisterBuildCallback(cfg.AddConsumersFromContainer);
                    // builder.RegisterBuildCallback(cfg.AddSagaStateMachinesFromContainer);
                    builder.RegisterBuildCallback(cfg.AddSagasFromContainer);
                    cfg.ConfigureMediator(
                        (context, configure) =>
                        {
                            var scope = context.GetRequiredService<ILifetimeScope>();
                            configure.UseMessageLifetimeScope(scope);
                            configure.UseInMemoryOutbox();

                            configure.ConfigureSagas(context);
                            configure.ConfigureConsumers(context);
                        }
                    );
                }
            );
        }
    }
}