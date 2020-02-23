using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Dotnet.Permissions.Server.Domain.Queries;
using Grpc.Dotnet.Permissions.V1;
using Grpc.Dotnet.Shared.Helpers.Rpc.Server;

namespace Grpc.Dotnet.Permissions.Server
{
    public class PermissionsServiceV1 : PermissionsService.PermissionsServiceBase
    {
        private readonly MessageOrchestrator messageOrchestrator;

        public PermissionsServiceV1(MessageOrchestrator messageOrchestrator)
        {
            this.messageOrchestrator = messageOrchestrator;
        }

        public override async Task<IsUserAllowedResponse> IsUserAllowed(IsUserAllowedRequest request, ServerCallContext context)
        {
            var isAllowed = await messageOrchestrator.Process<IsUserAllowedQuery, bool>(request, context);

            return new IsUserAllowedResponse { IsAllowed = isAllowed };
        }

        public override async Task<UserPermissionsResponse> GetUserPermissions(UserPermissionsRequest request, ServerCallContext context)
        {
            var permissions = await messageOrchestrator.Process<UserPermissionsQuery, IEnumerable<string>>(request, context);

            var response = new UserPermissionsResponse();
            response.Permissions.AddRange(permissions);

            return response;
        }
    }
}
