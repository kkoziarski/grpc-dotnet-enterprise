using System.IO;
using System.Threading.Tasks;
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
        private static bool dbCreated;
        private static object lockDbCreated = new object();

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

        public void MigrateTestDbAndSeed()
        {
            lock (lockDbCreated)
            {
                if (!dbCreated)
                {
                    using (var serviceScope = this.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
                    {
                        //serviceScope.ServiceProvider.GetRequiredService<TContext>().Database.Migrate();
                        SeedAsync(serviceScope).Wait();
                    }
                }
                else
                {
                    dbCreated = true;
                }
            }
        }

        private static Task SeedAsync(IServiceScope serviceScope)
        {
            //var seeder = serviceScope.ServiceProvider.GetRequiredService<IDataSeeder>();
            //await seeder.SeedAsync();
            return Task.CompletedTask;
        }
    }
}
