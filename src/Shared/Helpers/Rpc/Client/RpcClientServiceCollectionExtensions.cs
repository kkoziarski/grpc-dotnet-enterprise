namespace Grpc.Dotnet.Shared.Helpers.Rpc.Client
{
    using System.Configuration;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class RpcClientServiceCollectionExtensions
    {
        public static RpcClientBuilder AddRpcClients(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddScoped(typeof(IServiceClient<>), typeof(ServiceClient<>));

            var configurationSection = configuration.GetSection("RpcClients");

            if (!configurationSection.Exists())
            {
                throw new ConfigurationErrorsException("Cannot find configuration RpcClients section");
            }

            return new RpcClientBuilder(serviceCollection, configurationSection);
        }
    }
}