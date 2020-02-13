using Autofac;
using AutoMapper;
using Grpc.Dotnet.Notifications.Domain;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Grpc.Dotnet.Notifications.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMediatR(typeof(DomainModule).Assembly);
            services.AddAutoMapper(typeof(DomainModule));

            ////services.AddGrpc(); // this is configured in Program.cs
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new DomainModule());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMapper mapper)
        {
            app.UseRouting();

            if (env.IsDevelopment())
            {
                mapper.ConfigurationProvider.AssertConfigurationIsValid();
                app.UseDeveloperExceptionPage();
            }


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<NotificationServiceV1>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}
