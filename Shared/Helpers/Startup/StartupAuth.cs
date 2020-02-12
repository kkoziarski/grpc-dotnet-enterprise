namespace Grpc.Dotnet.Shared.Helpers.Startup
{
    using Grpc.Dotnet.Permissions.V1;
    using Grpc.Dotnet.Shared.Helpers.Authorization;
    using Grpc.Dotnet.Shared.Helpers.Rpc.Client;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class StartupAuth
    {
        public static IServiceCollection AddConfiguredAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAuthorizeService, AuthorizeService>();
            services.AddRpcClients(configuration)
                .AddClient<PermissionsService.PermissionsServiceClient>();

            return services;
        }
    }
}