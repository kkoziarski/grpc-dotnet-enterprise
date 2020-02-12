using Grpc.Dotnet.Shared.Helpers.Rpc.Client;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Moq;

namespace Grpc.Dotnet.Shared.Helpers.UnitTests.Helpers
{
    
    public class RpcClientMock<TClient> where TClient : ClientBase<TClient>
    {
        public RpcClientMock()
        {
            this.ClientMock = new Moq.Mock<TClient>();

            this.ServiceClient = new ServiceClient<TClient>(this.ClientMock.Object, new Mock<ILogger<TClient>>().Object);
        }

        public Mock<TClient> ClientMock { get; }

        public IServiceClient<TClient> ServiceClient { get; }
    }
}