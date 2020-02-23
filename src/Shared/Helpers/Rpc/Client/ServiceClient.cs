namespace Grpc.Dotnet.Shared.Helpers.Rpc.Client
{
    using System;
    using Google.Protobuf;
    using Google.Protobuf.WellKnownTypes;
    using Grpc.Core;
    using Microsoft.Extensions.Logging;

    public class ServiceClient<TClient> : IServiceClient<TClient>
        where TClient : ClientBase<TClient>
    {
        private readonly TClient client;
        private readonly ILogger<TClient> logger;

        public ServiceClient(TClient client, ILogger<TClient> logger)
        {
            this.client = client;
            this.logger = logger;
        }

        public void Execute<TRequest>(
            Func<TClient, Func<TRequest, CallOptions, Empty>> func,
            TRequest request,
            bool throwException = false)
            where TRequest : class, IMessage<TRequest>, new()
        {
            Execute<TRequest, Empty>(func, request, throwException);
        }

        public TResponse Execute<TRequest, TResponse>(
            Func<TClient, Func<TRequest, CallOptions, TResponse>> func,
            TRequest request,
            bool throwException = false)
            where TRequest : class, IMessage<TRequest>, new()
            where TResponse : class, IMessage<TResponse>, new()
        {
            var headers = new Metadata();

            var callOptions = new CallOptions(headers);

            TResponse response;

            try
            {
                logger.LogDebug($"{Environment.NewLine}GRPC Request{Environment.NewLine}Method: {func.Method.Name}{Environment.NewLine}");

                response = func(client)(request, callOptions);

                logger.LogDebug($"{Environment.NewLine}GRPC Response{Environment.NewLine}Method: {func.Method.Name}{Environment.NewLine}");
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Client executed with exception");
                if (throwException)
                {
                    throw;
                }

                return null;
            }

            return response;
        }
    }
}