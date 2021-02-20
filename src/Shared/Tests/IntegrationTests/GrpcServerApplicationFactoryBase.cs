using System.IO;
using System.Linq;
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

                var tContextServiceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(TContext));
                services.Remove(tContextServiceDescriptor);

                var dbContextOptionsDescriptor = services.FirstOrDefault(x => x.ServiceType == typeof(DbContextOptions<TContext>));
                services.Remove(dbContextOptionsDescriptor);

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
