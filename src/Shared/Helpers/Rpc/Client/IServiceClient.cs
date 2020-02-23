namespace Grpc.Dotnet.Shared.Helpers.Rpc.Client
{
    using System;
    using Google.Protobuf;
    using Google.Protobuf.WellKnownTypes;
    using Grpc.Core;

    public interface IServiceClient<out TClient>
        where TClient : ClientBase<TClient>
    {
        void Execute<TRequest>(
            Func<TClient, Func<TRequest, CallOptions, Empty>> func,
            TRequest request,
            bool throwException = false)
            where TRequest : class, IMessage<TRequest>, new();

        TResponse Execute<TRequest, TResponse>(
            Func<TClient, Func<TRequest, CallOptions, TResponse>> func,
            TRequest request,
            bool throwException = false)
            where TRequest : class, IMessage<TRequest>, new()
            where TResponse : class, IMessage<TResponse>, new();
    }
}