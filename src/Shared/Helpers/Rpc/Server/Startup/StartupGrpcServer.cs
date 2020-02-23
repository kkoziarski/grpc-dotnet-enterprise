namespace Grpc.Dotnet.Shared.Helpers.Rpc.Server.Startup
{
    using System.Configuration;
    using Grpc.Dotnet.Shared.Helpers.Rpc.Server;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Server.Kestrel.Core;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class StartupGrpcServer
    {
        public static IWebHostBuilder ConfigureGrpcServer(this IWebHostBuilder webBuilder)
        {
            webBuilder.ConfigureServices(services =>
            {
                services.AddGrpc(options =>
                {
                    options.Interceptors.Add<GlobalServerExceptionInterceptor>();
                    options.Interceptors.Add<GlobalServerLoggerInterceptor>();
                });

                services.AddSingleton<MessageOrchestrator>();
            });

            webBuilder.ConfigureKestrel((context, options) =>
            {
                var grpcPort = context.Configuration.GetValue<int>("RpcServer:Port", -1);
                if (grpcPort <= 0)
                {
                    throw new ConfigurationErrorsException("RpcServer.Port is missing");
                }

                options.ListenLocalhost(grpcPort, o => o.Protocols = HttpProtocols.Http2);
                options.ListenLocalhost(grpcPort - 100, o => o.Protocols = HttpProtocols.Http1);
            });

            return webBuilder;
        }
    }
}
