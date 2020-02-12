using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Dotnet.Permissions.Server.Domain.Queries;
using MediatR;

namespace Grpc.Dotnet.Permissions.Server.Domain.QueryHandlers
{
    public class UserAllowedQueryHandler : IRequestHandler<IsUserAllowedQuery, bool>
    {
        public Task<bool> Handle(IsUserAllowedQuery request, CancellationToken cancellationToken)
        {
            if (request.UserId == Guid.Empty || request.UserId == default(Guid))
            {
                return Task.FromResult(false);
            }

            // TODO: query database here
            if (request.Permission?.StartsWith("READ", StringComparison.OrdinalIgnoreCase) == true)
            {
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
    }
}
