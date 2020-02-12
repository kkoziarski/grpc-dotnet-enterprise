namespace Grpc.Dotnet.Shared.Helpers.Rpc.Server
{
    using System;
    using Autofac;
    using Grpc.Core;
    using Grpc.Core.Interceptors;
    using Microsoft.Extensions.Logging;

    public class RpcServerBuilder
    {
        private readonly ContainerBuilder containerBuilder;
        private readonly Grpc.Core.Server server;

        public RpcServerBuilder(ContainerBuilder containerBuilder, Grpc.Core.Server server)
        {
            this.containerBuilder = containerBuilder;
            this.server = server;
        }

        public RpcServerBuilder AddService<TServiceImpl>()
            where TServiceImpl : class
        {
            containerBuilder.RegisterType<TServiceImpl>()
                .AsSelf()
                .SingleInstance()
                .AutoActivate()
                .OnActivated(args =>
                {
                    var serviceImpl = args.Instance;

                    // Get service definition class for service implementation e.g.: definition TelemetryDataService for implementation TelemetryDataServiceV1
                    Type serviceDefinitionType = typeof(TServiceImpl).BaseType.DeclaringType;

                    // Get handle to static method BindService
                    var bindServiceMethod = serviceDefinitionType.GetMethod("BindService", new[] { typeof(TServiceImpl) });

                    // Invoke static method BindService from gRPC service definition e.g.: TelemetryDataService.BindService(serviceImpl);
                    var serverServiceDefinition = bindServiceMethod.Invoke(null, new object[] { serviceImpl }) as ServerServiceDefinition;

                    serverServiceDefinition = serverServiceDefinition.Intercept(new GlobalServerExceptionInterceptor(args.Context.Resolve<ILogger<GlobalServerExceptionInterceptor>>()));

                    serverServiceDefinition = serverServiceDefinition.Intercept(new GlobalServerLoggerInterceptor(args.Context.Resolve<ILogger<GlobalServerLoggerInterceptor>>()));

                    server.Services.Add(serverServiceDefinition);
                });

            return this;
        }
    }
}