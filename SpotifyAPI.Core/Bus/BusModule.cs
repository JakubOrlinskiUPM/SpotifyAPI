using System.Collections.Generic;
using Autofac;
using GreenPipes;
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

            builder.RegisterAssemblyTypes(assemblies)
                .As(typeof(ISendFilter))
                .AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(assemblies)
                .As(typeof(IPublishFilter))
                .AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(assemblies)
                .As(typeof(IConsumeFilter))
                .AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(assemblies)
                .As(typeof(ISendFilter<>))
                .AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(assemblies)
                .As(typeof(IConsumeFilter<>))
                .AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(assemblies)
                .As(typeof(IPublishFilter<>))
                .AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(assemblies)
                .As(typeof(IExecuteFilter<>))
                .AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(assemblies)
                .As(typeof(ICompensateFilter<>))
                .AsImplementedInterfaces();
            builder.RegisterAssemblyOpenGenericTypes(assemblies)
                .As(typeof(ISendFilter<>))
                .AsSelf();
            builder.RegisterAssemblyOpenGenericTypes(assemblies)
                .As(typeof(IConsumeFilter<>))
                .AsSelf();
            builder.RegisterAssemblyOpenGenericTypes(assemblies)
                .As(typeof(IPublishFilter<>))
                .AsSelf();
            builder.RegisterAssemblyOpenGenericTypes(assemblies)
                .As(typeof(IExecuteFilter<>))
                .AsSelf();
            builder.RegisterAssemblyOpenGenericTypes(assemblies)
                .As(typeof(ICompensateFilter<>))
                .AsSelf();


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
                    builder.RegisterBuildCallback(cfg.AddSagaStateMachinesFromContainer);
                    builder.RegisterBuildCallback(cfg.AddSagasFromContainer);
                    cfg.ConfigureMediator(
                        (context, configure) =>
                        {
                            var scope = context.GetRequiredService<ILifetimeScope>();
                            configure.UseMessageLifetimeScope(scope);
                            configure.UseInMemoryOutbox();

                            configure.ConfigureSend(
                                sendConfig => sendConfig.UseFilters(scope.Resolve<IEnumerable<ISendFilter>>())
                            );
                            configure.ConfigurePublish(
                                publishConfig => publishConfig.UseFilters(scope.Resolve<IEnumerable<IPublishFilter>>())
                            );
                            configure.UseFilters(scope.Resolve<IEnumerable<IConsumeFilter>>());
                            DependencyInjectionFilterExtensions.UseSendFilter(
                                configure,
                                typeof(SendFilterProcessor<>),
                                context
                            );
                            DependencyInjectionFilterExtensions.UseConsumeFilter(
                                configure,
                                typeof(ConsumeFilterProcessor<>),
                                context
                            );
                            DependencyInjectionFilterExtensions.UsePublishFilter(
                                configure,
                                typeof(PublishFilterProcessor<>),
                                context
                            );
                            DependencyInjectionFilterExtensions.UseExecuteActivityFilter(
                                configure,
                                typeof(ExecuteFilterProcessor<>),
                                context
                            );
                            DependencyInjectionFilterExtensions.UseCompensateActivityFilter(
                                configure,
                                typeof(CompensateFilterProcessor<>),
                                context
                            );

                            configure.ConfigureSagas(context);
                            configure.ConfigureConsumers(context);
                        }
                    );
                }
            );
        }
    }
}