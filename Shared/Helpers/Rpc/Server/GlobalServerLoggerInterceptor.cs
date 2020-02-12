namespace Grpc.Dotnet.Shared.Helpers.Rpc.Server
{
    using System;
    using System.Threading.Tasks;
    using Grpc.Core;
    using Grpc.Core.Interceptors;
    using Microsoft.Extensions.Logging;

    public class GlobalServerLoggerInterceptor : Interceptor
    {
        private readonly ILogger<GlobalServerLoggerInterceptor> logger;

        public GlobalServerLoggerInterceptor(ILogger<GlobalServerLoggerInterceptor> logger)
        {
            this.logger = logger;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            logger.LogDebug($"{Environment.NewLine}GRPC Request sent in method: {context.Method}{Environment.NewLine}");

            var response = await base.UnaryServerHandler(request, context, continuation);

            logger.LogDebug($"{Environment.NewLine}GRPC Response recieved in method: {context.Method}{Environment.NewLine}");

            return response;
        }
    }
}