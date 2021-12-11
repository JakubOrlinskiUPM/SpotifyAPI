using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SpotifyAPI.Core;
using SpotifyAPI.Core.Bootstrap;

namespace SpotifyAPI.API;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSpotifyAPI(Configuration);
        services.AddSwaggerGen(
            c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Nimbus", Version = "v1"});
                c.SchemaGeneratorOptions.SchemaIdSelector =
                    type => type.FullName?.Split('.').Last().Replace('+', '.');
                c.SupportNonNullableReferenceTypes();
                var xmlFiles = Directory
                    .GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly)
                    .ToList();
                xmlFiles.ForEach(xmlFile => c.IncludeXmlComments(xmlFile));

                var securityScheme = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer",
                    },
                    Scheme = "Bearer",
                    Type = SecuritySchemeType.OpenIdConnect,
                    OpenIdConnectUrl = new Uri(
                        $"{Configuration.GetValue<string>("BaseUrl")}{Configuration.GetSection("Identity").GetValue<string>("MetadataEndpoint")}"
                    ),
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                };

                c.AddSecurityDefinition("Bearer", securityScheme);
                c.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            securityScheme, new List<string> {"api"}
                        },
                    }
                );
                // c.EnableAnnotations();
            }
        );
    }

    public void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterInstance(Configuration).AsImplementedInterfaces().SingleInstance();
        builder.RegisterModule<CoreModule>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(
                c =>
                {
                    c.RoutePrefix = "swagger";
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SpotifyAPI v1");
                    c.EnableFilter();
                    c.EnableTryItOutByDefault();
                    c.EnableDeepLinking();
                }
            );
        }

        if (!env.IsDevelopment()) app.UseHttpsRedirection();

        app.UseRouting();

        app.UseNimbus(env);
    }
}