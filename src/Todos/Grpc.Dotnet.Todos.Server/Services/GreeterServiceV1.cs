using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Dotnet.Todos.Server.V1;
using Microsoft.Extensions.Logging;

namespace Grpc.Dotnet.Todos.Server
{
    public class GreeterServiceV1 : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterServiceV1> _logger;
        public GreeterServiceV1(ILogger<GreeterServiceV1> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }
    }
}
