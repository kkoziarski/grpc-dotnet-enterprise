using System.IO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Grpc.Dotnet.Shared.Helpers.IntegrationTests
{
    public class GrpcServerApplicationFactoryBase<TStartup, TContext> : WebApplicationFactory<TStartup>
        where TStartup : class
        where TContext : DbContext
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var projectDir = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(projectDir, "appsettings.integration.json");
            builder.UseEnvironment(Environments.Development);
            builder.ConfigureAppConfiguration((context, conf) =>
            {
                conf.AddJsonFile(configPath, optional: true);
                conf.AddEnvironmentVariables();
                this.ConfigureAppConfiguration(context, conf);
            });

            builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("Test", options => { });

                services.Replace(ServiceDescriptor.Singleton<TContext, TContext>());

                this.ConfigureTestServices(builder, services);
            });
        }

        protected virtual void ConfigureTestServices(IWebHostBuilder builder, IServiceCollection servicesConfiguration)
        {
        }

        protected virtual void ConfigureAppConfiguration(WebHostBuilderContext context, IConfigurationBuilder configuration)
        {
        }
    }
}
