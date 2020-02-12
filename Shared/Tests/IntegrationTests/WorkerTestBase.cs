using System;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Grpc.Dotnet.Shared.Helpers.IntegrationTests
{
    public class WorkerTestBase<TStartup, TContext, TFactory, TServiceClient> : IClassFixture<TFactory>
        where TStartup : class
        where TContext : DbContext
        where TFactory : WorkerApplicationFactoryBase<TStartup, TContext>
        where TServiceClient : ClientBase<TServiceClient>
    {
        protected TServiceClient Client { get; }

        public TContext DbContext { get; }

        public WorkerTestBase(TFactory factory)
        {

            factory.MigrateTestDbAndSeed();
            DbContext = factory.Services.GetRequiredService<TContext>();
            GrpcChannelOptions options = new GrpcChannelOptions
            {
                HttpClient = factory.CreateDefaultClient()
            };
            var channel = GrpcChannel.ForAddress("http://localhost", options);

            Client = (TServiceClient)Activator.CreateInstance(typeof(TServiceClient), new object[] { channel });
        }
    }
}
