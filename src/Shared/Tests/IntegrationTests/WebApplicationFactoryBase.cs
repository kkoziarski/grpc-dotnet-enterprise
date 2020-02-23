using System.IO;
using Grpc.Dotnet.Shared.Helpers.Rpc.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Grpc.Dotnet.Shared.Helpers.IntegrationTests
{
    public class WebApplicationFactoryBase<TStartup, TContext> : WebApplicationFactory<TStartup>
        where TStartup : class
        where TContext : class
    {
        protected override IHostBuilder CreateHostBuilder()
        {
            var builder = base.CreateHostBuilder();
            return builder;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var projectDir = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(projectDir, "appsettings.integration.json");

            builder.ConfigureAppConfiguration((context, conf) =>
            {
                conf.AddJsonFile(configPath, optional: true);
                conf.AddEnvironmentVariables();
            });

            builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("Test", options => { });

                services.Replace(ServiceDescriptor.Singleton<TContext, TContext>());

                services.AddSingleton(typeof(IServiceClient<>), typeof(ServiceClientMock<>));
            });
        }
    }
}
