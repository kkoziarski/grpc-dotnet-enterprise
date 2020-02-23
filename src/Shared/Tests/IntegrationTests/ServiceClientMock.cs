using System;
using System.Collections.Concurrent;
using System.Linq;
using Grpc.Dotnet.Shared.Helpers.Rpc.Client;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Shouldly;

namespace Grpc.Dotnet.Shared.Helpers.IntegrationTests
{
    public abstract class ServiceClientMockBase
    {
        // ReSharper disable once InconsistentNaming
        private static ConcurrentDictionary<string, int> requests { get; }

        protected static ConcurrentDictionary<string, int> Requests => requests;

        static ServiceClientMockBase()
        {
            requests = new ConcurrentDictionary<string, int>();
        }

        public static void ResetAllRequests()
        {
            requests.Clear();
        }
    }

    public class ServiceClientMock<T> : ServiceClientMockBase, IServiceClient<T>
        where T : ClientBase<T>
    {
        private T clientMock;

        public void SetMock(T mock)
        {
            clientMock = mock;
        }

        public void ClearMock()
        {
            clientMock = null;
        }

        public void Execute<TRequest>(Func<T, Func<TRequest, CallOptions, Empty>> func, TRequest request, bool throwException = false)
            where TRequest : class, IMessage<TRequest>, new()
        {
            Execute<TRequest, Empty>(func, request, throwException);
        }

        public TResponse Execute<TRequest, TResponse>(Func<T, Func<TRequest, CallOptions, TResponse>> func, TRequest request, bool throwException = false)
            where TRequest : class, IMessage<TRequest>, new()
            where TResponse : class, IMessage<TResponse>, new()
        {
            var requestName = GetRequestKey<TRequest>();
            if (Requests.ContainsKey(requestName))
            {
                Requests[requestName]++;
            }
            else
            {
                Requests.TryAdd(requestName, 1);
            }

            if (clientMock != null)
            {
                return func(clientMock)(request, new CallOptions());
            }

            return new TResponse();
        }

        public ServiceClientMock<T> EnsureRequest<TRequest>() where TRequest : IMessage<TRequest>
        {
            Requests.ShouldContainKey(GetRequestKey<TRequest>());
            return this;
        }

        public ServiceClientMock<T> EnsureRequestCount<TRequest>(int count) where TRequest : IMessage<TRequest>
        {
            Requests.TryGetValue(GetRequestKey<TRequest>(), out var numberOfRequests);
            numberOfRequests.ShouldBe(count, $"Requests of type: {typeof(TRequest).Name}");

            return this;
        }

        public ServiceClientMock<T> EnsureAllRequestsCount(int count)
        {
            var numberOfAllRequests = Requests.Where(d => d.Key.StartsWith($"{typeof(T).Name}_")).Select(d => d.Value).Sum();

            numberOfAllRequests.ShouldBe(count, $"Requests of client type: {typeof(T).Name}");
            return this;
        }

        private static string GetRequestKey<TRequest>()
        {
            return $"{typeof(T).Name}_{typeof(TRequest).Name}";
        }
    }
}
