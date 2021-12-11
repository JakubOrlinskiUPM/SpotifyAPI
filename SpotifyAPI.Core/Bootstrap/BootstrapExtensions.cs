using FluentValidation.AspNetCore;
using Hangfire;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Opw.HttpExceptions.AspNetCore;
using Opw.HttpExceptions.AspNetCore.Mappers;
using SpotifyAPI.Core.Database;
using SpotifyAPI.Core.Identity.Entities;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;

namespace SpotifyAPI.Core.Bootstrap;

public static class BootstrapExtensions
    {
        public static void AddSpotifyAPI (this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDb>(
                options =>
                    options.UseNpgsql(
                            configuration.GetConnectionString("AppDb"),
                            builder =>
                                builder.MigrationsAssembly("SpotifyAPI.Migrations")
                        )
                        .EnableDetailedErrors()
                        .EnableSensitiveDataLogging()
            );

            services.AddIdentity<User, Role>(
                    options =>
                    {
                        options.SignIn.RequireConfirmedAccount = false; // TODO: Email confirmation
                        options.User.RequireUniqueEmail = true;
                    }
                )
                .AddEntityFrameworkStores<AppDb>()
                .AddDefaultTokenProviders();


            services.AddIdentityServer(
                    options => { options.Cors.CorsPolicyName = "Default"; }
                )
                .AddAspNetIdentity<User>()
                .AddOperationalStore<AppDb>()
                .AddJwtBearerClientAuthentication()
                .AddInMemoryClients(
                    new List<Client>
                    {
                        new()
                        {
                            ClientId = "Development",
                            ClientSecrets = new[] { new Secret("Development".Sha256()) },
                            ClientName = "Development",
                            AllowedScopes = new[] { "api", "openid", "profile" },
                            AllowedGrantTypes = new[]
                                { OidcConstants.GrantTypes.ClientCredentials, OidcConstants.GrantTypes.Password },
                            RequireClientSecret = false,
                            AllowedCorsOrigins = IdentityCorsPolicyService.AllowedOrigins,
                        },
                        new()
                        {
                            ClientId = "Public",
                            ClientName = "Public",
                            AllowedScopes = new[] { "api" },
                            RequireClientSecret = false,
                        },
                        new()
                        {
                            ClientId = "Local",
                            ClientName = "Local",
                            AllowedScopes = new[] { "api" },
                            AllowedGrantTypes = new[]
                                { OidcConstants.GrantTypes.ClientCredentials },
                            RequireClientSecret = false,
                        },
                    }
                )
                .AddInMemoryApiScopes(
                    new List<ApiScope>
                    {
                        new("api", new[] { JwtClaimTypes.Role }),
                    }
                )
                .AddInMemoryApiResources(
                    new List<ApiResource>
                    {
                        new("api"),
                    }
                )
                .AddInMemoryIdentityResources(
                    new List<IdentityResource>
                    {
                        new IdentityResources.OpenId(),
                        new IdentityResources.Profile(),
                        new IdentityResources.Email(),
                    }
                )
                .AddDeveloperSigningCredential(signingAlgorithm: IdentityServerConstants.RsaSigningAlgorithm.RS512)
                .AddCorsPolicyService<IdentityCorsPolicyService>();

            services.AddAuthentication(
                    options =>
                    {
                        options.DefaultAuthenticateScheme = IdentityServerJwtConstants.IdentityServerJwtScheme;
                    }
                )
                .AddLocalApi(IdentityServerJwtConstants.IdentityServerJwtScheme, _ => { });

            services.AddAuthorization(
                o =>
                {
                    o.DefaultPolicy = new AuthorizationPolicyBuilder(IdentityServerJwtConstants.IdentityServerJwtScheme)
                        .RequireAuthenticatedUser()
                        .RequireRole(Role.User, Role.Agent)
                        .Build();
                }
            );

            services.AddMvc()
                .AddFluentValidation()
                .AddControllersAsServices()
                .AddMvcOptions(
                    options =>
                    {
                        options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
                    }
                )
                .AddHttpExceptions(
                    options =>
                    {
                        options.ExceptionMapper<RequestException, RequestExceptionMapper>();
                        options.ExceptionMapper<RequestFaultException, RequestExceptionMapper>();
                        options.ExceptionMapper<Exception, ProblemDetailsExceptionMapper<Exception>>();
                    }
                );
            services.AddFluentValidationRulesToSwagger();

            services.AddCors();
            services.AddHttpContextAccessor();

            services.AddHangfire(
                _ =>
                {
                    _.UseRecommendedSerializerSettings(
                        settings => { settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore; }
                    );
                    _.UseSimpleAssemblyNameTypeSerializer();
                    _.UseRedisStorage(configuration.GetConnectionString("Redis"));
                }
            );
            services.AddHangfireServer();
        }

        public static void UseNimbus (this IApplicationBuilder app, IWebHostEnvironment env)
        {
            var config = app.ApplicationServices.GetRequiredService<IConfiguration>();

            if (env.IsDevelopment())
            {
                app.UseHangfireDashboard();
            }

            app.UseRouting();
            app.UseCors(
                builder => builder
                    .WithOrigins(config.GetSection("AllowedOrigins").Get<string[]>())
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
            );
            app.UseAuthentication();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseHttpExceptions();

            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapGet(
                        "/",
                        context =>
                        {
                            context.Response.Redirect("/docs", true);
                            return Task.CompletedTask;
                        }
                    );
                }
            );
        }
    }