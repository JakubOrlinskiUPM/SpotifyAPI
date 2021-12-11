using Autofac;
using Microsoft.AspNetCore.Authorization;

namespace SpotifyAPI.Core.Identity
{
    public class IdentityModule : Module
    {
        protected override void Load (ContainerBuilder builder)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // builder.RegisterType<AuthorizationHandlerProvider>()
            //     .As<IAuthorizationHandlerProvider>()
            //     .SingleInstance();
            //
            // builder.RegisterType<AuthorizationService>()
            //     .As<IAuthorizationService>()
            //     .SingleInstance();

            builder.RegisterAssemblyTypes(assemblies)
                .AsClosedTypesOf(typeof(AuthorizationHandler<>))
                .As<IAuthorizationHandler>()
                .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(assemblies)
                .AsClosedTypesOf(typeof(AuthorizationHandler<,>))
                .As<IAuthorizationHandler>()
                .InstancePerLifetimeScope();
        }
    }
}