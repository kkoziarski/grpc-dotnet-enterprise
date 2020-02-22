using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Dotnet.Permissions.Domain;
using Grpc.Dotnet.Permissions.Server.Domain.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Grpc.Dotnet.Permissions.Server.Domain.QueryHandlers
{
    public class UserPermissionsQueryHandler : IRequestHandler<UserPermissionsQuery, IEnumerable<string>>
    {
        private readonly ILogger<UserPermissionsQueryHandler> logger;

        public UserPermissionsQueryHandler(ILogger<UserPermissionsQueryHandler> logger)
        {
            this.logger = logger;
        }

        public Task<IEnumerable<string>> Handle(UserPermissionsQuery request, CancellationToken cancellationToken)
        {
            var permissions = Enumerable.Empty<string>();

            // TODO: query database here
            if (request.UserId == DomainModule.AdminId)
            {
                permissions = new string[]
                {
                    "READ",
                    "CREATE",
                    "DELETE",
                    "UPDATE"
                };
            }
            else
            {
                permissions = new string[]
                {
                    "READ",
                    "CREATE"
                };
            }

            logger.LogInformation($"User permissions: {string.Join(",", permissions)}");

            return Task.FromResult(permissions);
        }
    }
}
