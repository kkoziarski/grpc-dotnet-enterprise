namespace Grpc.Dotnet.Shared.Helpers.Rpc.Client
{
    using System;
    using System.Configuration;
    using Grpc.Core;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.DependencyInjection;

    public class RpcClientBuilder
    {
        private readonly IServiceCollection serviceCollection;
        private readonly IConfigurationSection configurationSection;

        public RpcClientBuilder(IServiceCollection serviceCollection, IConfigurationSection configurationSection)
        {
            this.serviceCollection = serviceCollection;
            this.configurationSection = configurationSection;
        }

        public RpcClientBuilder AddClient<TClient>()
            where TClient : ClientBase<TClient>
        {
            var clientType = typeof(TClient);
            var clientName = clientType.Name;
            var clientConfigurationSection = configurationSection.GetSection(clientName);

            if (!clientConfigurationSection.Exists())
            {
                throw new ConfigurationErrorsException($"Cannot find configuration section for rpcClient:{clientName}");
            }

            var host = clientConfigurationSection.GetValue<string>("Host");
            var port = clientConfigurationSection.GetValue<int>("Port");

            serviceCollection.AddSingleton<TClient>(serviceProvider =>
            {
                var logger = serviceProvider.GetRequiredService<ILogger<TClient>>();
                logger.LogInformation($"Rpc Client:{clientName} is connecting to {host}:{port}");

                var channel = new Channel(host, port, ChannelCredentials.Insecure);

                //var tracer = serviceProvider.GetRequiredService<ITracer>();
                //var rpcClientInterceptor = new ClientTracingInterceptor.Builder(tracer)
                //    .WithTracedAttributes(ClientTracingConfiguration.RequestAttribute.Headers, ClientTracingConfiguration.RequestAttribute.MethodName, ClientTracingConfiguration.RequestAttribute.AllCallOptions, ClientTracingConfiguration.RequestAttribute.MethodType)
                //    .Build();
                //var invoker = channel.Intercept(rpcClientInterceptor);

                var invoker = channel.CreateCallInvoker();
                var instance = Activator.CreateInstance(clientType, invoker) as TClient;
                return instance;
            });

            return this;
        }
    }
}