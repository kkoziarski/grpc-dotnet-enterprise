using System;
using System.Net.Http;
using Grpc.Dotnet.Shared.Helpers.Rpc.Client;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Grpc.Dotnet.Shared.Helpers.IntegrationTests
{
    public abstract class ControllerTestBase<TStartup, TContext> : IClassFixture<WebApplicationFactoryBase<TStartup, TContext>>, IDisposable
        where TStartup : class
        where TContext : class
    {
        protected WebApplicationFactory<TStartup> Factory { get; }

        protected ControllerTestBase(WebApplicationFactoryBase<TStartup, TContext> factory)
        {
            ServiceClientMockBase.ResetAllRequests();

            this.Factory = factory;
        }

        public TContext DbContext => Factory.Services.GetRequiredService<TContext>(); ////Factory.Server.Host.Services.GetRequiredService<TContext>();

        public HttpClient CreateClient()
        {
            return Factory.CreateClient();
        }

        public ServiceClientMock<TClient> GetRpcClientMock<TClient>() where TClient : ClientBase<TClient>
        {
            return Factory.Services.GetRequiredService<IServiceClient<TClient>>() as ServiceClientMock<TClient>;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServiceClientMockBase.ResetAllRequests();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
