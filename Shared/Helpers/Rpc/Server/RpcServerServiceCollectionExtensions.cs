namespace Grpc.Dotnet.Shared.Helpers.Rpc.Server
{
    using System.Configuration;
    using Autofac;
    using Grpc.Core;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;

    public static class RpcServerServiceCollectionExtensions
    {
        public static RpcServerBuilder RegisterRpcServerHostService(this ContainerBuilder containerBuilder, HostBuilderContext builderContext)
        {
            var configurationSection = builderContext.Configuration.GetSection("RpcServer");

            if (!configurationSection.Exists())
            {
                throw new ConfigurationErrorsException("Cannot find configuration RpcServer section");
            }

            var host = configurationSection.GetValue<string>("Host");
            if (string.IsNullOrWhiteSpace(host))
            {
                throw new ConfigurationErrorsException("RpcServer.Host is missing");
            }

            var port = configurationSection.GetValue<int>("Port", -1);
            if (port <= 0)
            {
                throw new ConfigurationErrorsException("RpcServer.Port is missing");
            }

            containerBuilder.RegisterType<MessageOrchestrator>().AsSelf().SingleInstance();

            var server = new Grpc.Core.Server();
            server.Ports.Add(new ServerPort(host, port, ServerCredentials.Insecure));

            containerBuilder.RegisterInstance(server).SingleInstance();

            containerBuilder.RegisterType<RpcServerHostedService>().As<IHostedService>().InstancePerDependency();

            return new RpcServerBuilder(containerBuilder, server);
        }
    }
}