using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Grpc.Dotnet.Shared.Helpers.IntegrationTests
{
    public class GrpcServerTestBase<TStartup, TContext, TFactory, TServiceClient> : IClassFixture<TFactory>
        where TStartup : class
        where TContext : DbContext
        where TFactory : GrpcServerApplicationFactoryBase<TStartup, TContext>
        where TServiceClient : ClientBase<TServiceClient>
    {
        protected TServiceClient Client { get; }

        public TContext DbContext { get; }

        public GrpcServerTestBase(TFactory factory)
        {
            factory.MigrateTestDbAndSeed();
            DbContext = factory.Services.GetRequiredService<TContext>();

            // FIXING
            // BlockingUnaryCall fails with Bad gRPC response if no TLS/SSL is used #654 https://github.com/grpc/grpc-dotnet/issues/654
            // Status(StatusCode=Internal, Detail="Bad gRPC response. Response protocol downgraded to HTTP/1.1." #682
            // Tester example project FunctionalTests fail with Bad gRPC response. https://github.com/grpc/grpc-dotnet/issues/648#issuecomment-561459918
            var httpClient = factory.CreateDefaultClient(new ResponseVersionHandler());

            GrpcChannelOptions options = new GrpcChannelOptions
            {
                HttpClient = httpClient,
            };
            var channel = GrpcChannel.ForAddress(httpClient.BaseAddress, options);

            Client = (TServiceClient)Activator.CreateInstance(typeof(TServiceClient), new object[] { channel });
        }

        private class ResponseVersionHandler : DelegatingHandler
        {
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var response = await base.SendAsync(request, cancellationToken);
                response.Version = request.Version;

                return response;
            }
        }
    }
}
