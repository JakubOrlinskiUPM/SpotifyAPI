using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace SpotifyAPI.API;

public class Program
{
    public static void Main (string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder (string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureWebHostDefaults(
                webBuilder =>
                {
                    webBuilder.ConfigureAppConfiguration(
                        (context, builder) =>
                        {
                            builder.AddJsonFile("appsettings.Local.json", true);
                            builder.AddJsonFile(
                                $"appsettings.{context.HostingEnvironment.EnvironmentName}.Local.json",
                                true
                            );

                            // if (context.HostingEnvironment.IsDevelopmentOrTesting())
                            //     builder.AddUserSecrets<Program>(true);
                        }
                    );
                    webBuilder.UseStartup<Startup>();
                }
            );
    }
}