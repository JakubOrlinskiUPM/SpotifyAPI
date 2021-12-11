using Autofac;
using Hangfire;
using Mapster;
using MapsterMapper;
using SpotifyAPI.Core.Bus;
using SpotifyAPI.Core.Database;
using SpotifyAPI.Core.Identity;
using SpotifyAPI.Core.Validation;
using Opw.HttpExceptions.AspNetCore.Mappers;
using SpotifyAPI.Core.Modules.Playlist;


namespace SpotifyAPI.Core
{
    public class CoreModule : Module
    {
        protected override void Load (ContainerBuilder builder)
        {
            builder.RegisterModule<DatabaseModule>();
            builder.RegisterModule<BusModule>();
            builder.RegisterModule<IdentityModule>();
            builder.RegisterModule<ValidationModule>();

            builder.RegisterType<Mapper>()
                .AsImplementedInterfaces();

            TypeAdapterConfig.GlobalSettings.Default.IgnoreNullValues(true);

            builder.RegisterBuildCallback(
                scope => GlobalConfiguration.Configuration.UseAutofacActivator(scope.BeginLifetimeScope())
            );

            builder.RegisterAssemblyOpenGenericTypes(ThisAssembly);

            builder.RegisterGeneric(typeof(ProblemDetailsExceptionMapper<>))
                .AsSelf();

            builder.RegisterModule<PlaylistModule>();
        }
    }
}