using Grpc.Dotnet.Shared.Helpers.Authorization;

namespace Grpc.Dotnet.Shared.Helpers.IntegrationTests
{
    public class AuthorizeServiceMock : IAuthorizeService
    {
        public bool IsCurrentUserAuthorized(string permission)
        {
            return true;
        }
    }
}