namespace Grpc.Dotnet.Shared.Helpers.Rpc.Server
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public class RpcServerHostedService : BackgroundService
    {
        private readonly Grpc.Core.Server server;
        private readonly ILogger<RpcServerHostedService> logger;

        public RpcServerHostedService(Grpc.Core.Server server, ILogger<RpcServerHostedService> logger)
        {
            this.server = server;
            this.logger = logger;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogDebug($"RpcServerHostService background task is stopping");

            await server.ShutdownAsync().ConfigureAwait(false);

            logger.LogDebug($"RpcServerHostService background has stopped");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogDebug($"RpcServerHostService is starting");

            try
            {
                server.Start();
                foreach (var serverPort in server.Ports)
                {
                    logger.LogInformation($"{Environment.NewLine}Rpc Server is listening on {serverPort.Host}:{serverPort.Port}");
                }
            }
            catch (Exception exception)
            {
                logger.LogError(exception, $"Error in {nameof(ExecuteAsync)}");
                throw;
            }

            logger.LogDebug($"RpcServerHostService has started");

            return Task.CompletedTask;
        }
    }
}
