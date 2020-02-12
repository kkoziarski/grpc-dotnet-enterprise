using Grpc.Dotnet.Todos.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MediatR;
using AutoMapper;
using Grpc.Dotnet.Shared.Helpers.Startup;
using Grpc.Dotnet.Shared.Helpers.Rpc.Client;
using Grpc.Dotnet.Todos.Notification.V1;
using Grpc.Dotnet.Permissions.V1;
using Autofac;

namespace Grpc.Dotnet.Todos.Api
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
            services.AddDbContext<TodoDbContext>(options => options.UseInMemoryDatabase("Todos"));
            services.AddMediatR(typeof(DomainModule).Assembly);
            services.AddAutoMapper(typeof(Startup), typeof(DomainModule));

            services.AddConfiguredAuthentication(this.Configuration);
            services.AddConfiguredUserContext();

            services.AddRpcClients(this.Configuration)
                .AddClient<NotificationService.NotificationServiceClient>()
                .AddClient<PermissionsService.PermissionsServiceClient>();

            services.AddMvc();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new ApiModule());
            builder.RegisterModule(new DomainModule());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMapper mapper)
        {
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            if (env.IsDevelopment())
            {
                mapper.ConfigurationProvider.AssertConfigurationIsValid();
                app.UseDeveloperExceptionPage();
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
                endpoints.MapControllers(); // Map attribute-routed API controllers
                endpoints.MapDefaultControllerRoute(); // Map conventional MVC controllers using the default route
            });
        }
    }
}
