using Grpc.Core;
using Grpc.Dotnet.Shared.Helpers.IntegrationTests;
using Microsoft.EntityFrameworkCore;

namespace Grpc.Dotnet.Permissions.Server.IntegrationTests
{
    public class PermissionsServerTestBase<TServiceClient> : GrpcServerTestBase<Startup, DbContext, PermissionsServerApplicationFactory, TServiceClient>
        where TServiceClient : ClientBase<TServiceClient>
    {
        public PermissionsServerTestBase(PermissionsServerApplicationFactory factory) : base(factory)
        {
        }
    }
}
