using Grpc.Dotnet.Shared.Helpers.IntegrationTests;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Grpc.Dotnet.Permissions.Server.IntegrationTests
{
    public class PermissionsServerApplicationFactory : GrpcServerApplicationFactoryBase<Startup, DbContext>
    {
        protected override void ConfigureTestServices(IWebHostBuilder builder, IServiceCollection services)
        {
            base.ConfigureTestServices(builder, services);
            services.AddDbContext<DbContext>(options => options.UseInMemoryDatabase("TodosIntegrationTests"));
            //services.AddScoped<IMyService, MyService>();
        }
    }
}