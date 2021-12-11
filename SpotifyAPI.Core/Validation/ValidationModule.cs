using Autofac;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace SpotifyAPI.Core.Validation
{
    public class ValidationModule : Module
    {
        protected override void Load (ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(ThisAssembly)
                .AsSelf()
                .AsClosedTypesOf(typeof(IValidator<>));

            builder.RegisterAssemblyOpenGenericTypes(ThisAssembly)
                .AssignableTo(typeof(IValidator<>))
                .AsSelf()
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(ThisAssembly)
                .AssignableTo(typeof(IValidatorInterceptor))
                .As(typeof(IValidatorInterceptor));
        }
    }
}